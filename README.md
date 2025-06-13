# IncreasePBS Script  

## Overview  
The `IncreasePBS` script is a DataMiner Automation script designed to manage and update the PBS (Product Backlog Score) values of tasks in a Task Manager element. It identifies tasks in specific states and adjusts their PBS values based on predefined rules.  

## Features  
- Filters tasks based on their state (`In Progress`, `Code Review`, `Quality Assurance`).  
- Updates PBS values for tasks:  
	- Sets a base PBS value if the current PBS is below the threshold.  
	- Increments PBS by 10 for tasks meeting the criteria.
- Updates the estimated time for tasks with a default `ttid` value (4 days). 
- Includes safeguards to avoid overwhelming the system by introducing a delay between updates.  

## Script Details  

### Entry Point  
The script starts execution in the `Run` method, which handles exceptions gracefully and calls the `RunSafe` method for the main logic.  

### Main Logic (`RunSafe`)  
1. **Base PBS Value**:  
 - The script starts with a base PBS value of `500`.  

2. **Task Filtering**:  
 - Filters tasks in the table with PID `120` based on the following states:  
   - `In Progress`  
   - `Code Review`  
   - `Quality Assurance`  

3. **PBS Update Rules**:  
 - If the current PBS is less than the base PBS,
	- it sets the PBS to the base value.
	- Updates the estimated time for tasks with a valid `ttid` to 4 days.
 - Otherwise, increments the PBS by `10`.  

4. **System Safeguard**:  
 - Introduces a `2-second` delay between updates to avoid overwhelming the system.  

## Usage  
- Deploy the script in the DataMiner Automation environment.  
- Ensure the table with PID `100` and column PID `120` exists in the target element.  
- Schedule the script to run periodically if PBS needs to be incremented regularly.  

## Notes  
- The script includes commented-out sections for alternative PBS calculation methods, such as using task age.  
- Exception handling ensures the script exits gracefully in case of errors.
