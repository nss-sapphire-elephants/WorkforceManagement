#BangazonWorkforceSapphireElephants

## Software Requirements
Sql Server Manangment Studio
Visual Studio Community 2017
## Enitity Relationship Diagram
![alt text](https://i.imgur.com/z0rFYz0.png)

### Description
Bangazon Workforce Management supports an HR department with hiring, equipment allocation and personal development opportunities within the Bangazon company.

### 1. Employee

 ##### GET
Given an HR employee wants to view employees
when the employee clicks on the "Employees" item in the navigation bar,
all current employees should be listed with the following information:

1. First name and last name
2. Department

 ##### POST
Given the user is viewing the list of employees
when the user clicks the "Create New link" hyperlink,
a form for will be displayed on which the following information can be entered:

1. First name
2. Last name
3. Is the employee a supervisor
4. Select a department from a drop down

##### Detail
Given a user is viewing the employee list,
when the user clicks on an individual employee,
the user should be shown a detail view of that employee including:

1. First name and last name
2. Department
3. Currently assigned computer
4. Training programs they have attended, or plan on attending
 
 ##### Edit
Given user is viewing an employee
When user clicks on the "Edit" link
Then user should be able to edit the name of the employee, or change the department to which the employee is assigned


### 2. Department

 ##### GET
Given user wants to view departments,
when user clicks on the 'Departments' section in the navigation bar,
all current departments should be listed
and the following information should be presented to the user:

1. Department name
2. Department budget
3. Size of department (number of employees assigned)
 
 ##### Create New
Given user is viewing all departments,
when user clicks on the "Create New" link
a form should be presented in which the new department name can be entered

##### Detail
Given user is viewing list of departments,
when a user clicks on a department
a view should be presented with the department name as a header,
a list of employees currently assigned to that department should be listed
Given the user wants to see all of the employees in a department
When the user performs a gesture on the department detail affordance
Then the user should see the department name
And the user should see the department budget
And the user should see the full name of each employee in that department


### 4. TrainingProgram

 ##### GET
Given a user wants to view all training programs
When the user clicks the Training Programs item in the navigation bar
Then the user will see a list of all training programs that have not taken place yet

Given the user is viewing all training programs
When the user clicks the Create New link
Then the user should be presented with a form in which the following information can be entered

1. Name
2. Title
3. Start day
4. End day
5. Maximum number of attendees

##### DETAILS and EDIT 
Given user is viewing the list of training programs
When the user clicks on a training program
Then the user should see all details of that training program
And any employees that are currently attending the program

Given user is viewing the details of a training program
When the user clicks on the edit link
Then the user should be presented with a form that allows the user to edit any property of the training program unless the training program has already taken place

##### DELETE
Given user wants to remove a training program
When the user clicks on a training program
And then clicks on the delete button
Then the training program should be deleted unless the training program has already taken place

##### PAST PROGRAMS
Given the user wants to see past training programs
When the user is views the list of future training programs
Then the user should have an affordance labeled "View Past Programs" at the end of the list

Given the user is viewing the list of future training programs
When the user performs a gesture of the past programs affordance
Then the user should see a list of all training programs that ended before the current day

###### DETAILS OF PAST PROGRAMS
Given the user wants to see past training programs
When the user is views the list of future training programs
Then the user should have an affordance labeled "View Past Programs" at the end of the list

Given the user is viewing the list of future training programs
When the user performs a gesture of the past programs affordance
Then the user should see a list of all training programs that ended before the current day

### 5. Computer
Given a user wants to view all computers
When the user clicks on the Computers item in the navigation bar
Then the user should see a list of all computers
And each item should be a hyperlink that can be clicked to view the details

Given a user is viewing all computers
When the user clicks the Create New link
Then the user should be presented with a form in which the following information can be entered

Computer manufacturer
Computer make
Purchase date
Given user is viewing a single computer
When the user clicks on the Delete link
Then the user should be presented with a screen to verify that it should be deleted
And if the user chooses Yes from that screen, the computer should be deleted only if it is has never been assigned to an employee

###### ASSIGN
Given a user wants to assign a new computer to an employee
When the user clicks on the create computer affordance

###### SEARCH
Given a user wants to view specific computers by make or manufacturer
When the user views the computer list
Then the user should have an affordance to type in the name of a computer make or computer manufacturer

Given the user has entered in the name of a computer make || manufacturer in the computer filter affordance
When the user presses the enter key
Then the computer list should be re-rendered with computers that matches the make || manufacturer
Then the creation form should include an affordance that lists all employees

Given the user is viewing the computer creation for
When the user fills out all properties for the computer
And has chosen an employee
Then the user should be redirected back to the list of computers

Given the user has created a new computer
When the user is redirected to the computer list
Then the currently assigned employee's full name should be displayed in the list
