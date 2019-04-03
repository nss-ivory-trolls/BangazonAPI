## Http Request Methods
## 1. Customer
Start the program by cd'ing into the BangazonAPI and using the command dotnet run. Once the program is running, open up the Postman desktop app and run the following commands:
### GET
- select GET then paste localhost:5000/customer into the field and click send. The result should be an array of all the customers in the database.

- select GET then paste http://localhost:5000/customer?_include=payments into the field and click send. The result should be an array of all the customers in the database with all of the payment types included in that customers as well.

- select GET then paste http://localhost:5000/customer?_include=products into the field and click send. The result should be an array of all the customers in the database with all of the products types included in that customers as well.

- select GET then paste http://localhost:5000/customer?q=sat into the field and click send. The result should be an array of all the customers in the database with first or last names that contains sat.

- select GET then paste localhost:5000/customer/1 or any other number that showed up in the previous query as CustomerId and click send. The result should be only that object of the specified Customer

### POST
select POST, then paste localhost:5000/customer into the field, then click Body underneath the field, then select raw, and then paste this snippet or make one similar
``` {
        "FirstName": "Test",
        "LastName": "Customer"
    } ```
then click send. The result should be the new customer you made.

### PUT
select PUT then paste localhost:5000/customer/1 or any other Customer Id , then click Body underneath the field, then select raw, and then paste this snippet or make one similar
``` {
        "FirstName": "Test",
        "LastName": "Instructions"
    } ```
You should get nothing back from this. When you run the GET query the Customer you specified in your PUT query should show the updated, edited information you gave it.

