

namespace IncreasePBS
{

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
    {
        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
            try
            {
                RunSafe(engine);
            }
            catch (ScriptAbortException)
            {
                // Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
                throw; // Comment if it should be treated as a normal exit of the script.
            }
            catch (ScriptForceAbortException)
            {
                // Catch forced abort exceptions, caused via external maintenance messages.
                throw;
            }
            catch (ScriptTimeoutException)
            {
                // Catch timeout exceptions for when a script has been running for too long.
                throw;
            }
            catch (InteractiveUserDetachedException)
            {
                // Catch a user detaching from the interactive script by closing the window.
                // Only applicable for interactive scripts, can be removed for non-interactive scripts.
                throw;
            }
            catch (Exception e)
            {
                engine.ExitFail("Run|Something went wrong: " + e);
            }
        }

        private void RunSafe(IEngine engine)
        {
            int basePBS = 500; // Base PBS value to start from
			IDms dms = engine.GetDms();

			Element elementEngine = engine.FindElementsByName(engine.GetDummy(1).ElementName).FirstOrDefault();
			IDmsElement elementTaskmanager = dms.GetElement(engine.GetDummy(1).ElementName);
			IDmsTable tableTasks = elementTaskmanager.GetTable(100);

            var filters = new List<ColumnFilter> {
                   new ColumnFilter{
                       Pid = 120, // PID of the column State  
                       ComparisonOperator = ComparisonOperator.Equal,
                       Value = "In Progress",
                   },
                   new ColumnFilter
                   {
                       Pid = 120, // PID of the column State
                       ComparisonOperator = ComparisonOperator.Equal,
                       Value = "Code Review",
                   },
                   new ColumnFilter
                   {
                       Pid = 120, // PID of the column State
                       ComparisonOperator = ComparisonOperator.Equal,
                       Value = "Quality Assurance",
                   },

               };

            List<object[]> matchingRows = tableTasks.QueryData(filters).ToList();

			foreach (var row in matchingRows)
			{
				// Increase the PBS of the task by 1  
				int currentPBS = Convert.ToInt32(row[4]);
				int ttid = Convert.ToInt32(row[8]);

				if (currentPBS < basePBS)
				{
					// engine.GenerateInformation("Task " + row[2] + " has a PBS of " + pbs + ", which is less than the base PBS of " + basePBS + ". Setting base PBS for this.");

					elementEngine.SetParameterByPrimaryKey(154, Convert.ToString(row[0]), basePBS);

					engine.Sleep(2000);

					if (ttid != -1)
					{
						elementEngine.SetParameterByPrimaryKey(158, Convert.ToString(row[0]), 32); // Set to 4 days, 85% of dev tasks in last 6 months is done in less then 3.5 days
						engine.Sleep(2000); // Sleep for 2 second to avoid overwhelming the system

					}
				}
				else
				{

					/* Use the below if PBS needs to be set to new BPS taking the workflow age is int account.
					int age = Convert.ToInt32(Math.Floor(Convert.ToDouble(row[37]))); // Convert age to integer by removing decimal precision
					int newPBS = currentPBS + (10 * age); 
					*/

					/*
					 * Use the below if PBS needs to be incremented with 10. Use a scheduled task to exucte this script as often as the PBS needs to be incremented.
					 */
					int newPBS = currentPBS + 10;
												  												  				

					elementEngine.SetParameterByPrimaryKey(154, Convert.ToString(row[0]), newPBS);
					engine.Sleep(2000); // Sleep for 2 second to avoid overwhelming the system
				}

				
			}

			

			//engine.GenerateInformation(matchingRows.Count + " tasks are currently in progress, code review or quality assurance.");
		}
    }
}
