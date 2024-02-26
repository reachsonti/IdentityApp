1) Restore the AuthSystemDB into sql server.
2) Copy the application to your local machine folder.
3) Change the details in appsettings for database connection. Below is mine.
   "IdentityDBContextConnection": "Server=localhost;Database=AuthSystemDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
4) Open the application in Visual Studio IDE and run the application.
5) Click on the Register to register new users. DB table is ASPNetUsers. Kindly note there is no Email functionality available.
6) Login into the application with the credentials.
7) Click on the email link on the menu available on top right. It will open up the page showing the links to Manage your account.
8) Click on the Profile link to update the information. Available fields are
     a) FirstName
     b) SecondName
     c) Username
     d) PhoneNumebr
     c) Profile Image
     e) Upload Document
   Select the profileimage from local machine and also the document if nessesary then click on the save button.
   Images are stored in wwwroot/images folder.
   Documents are stored in wwwroot/documents folder.
   You can view the table below, showing the image name, image and delete button. Click on the delete button to delete the image.
   You can view the download button below the Upload Document field. Click on the download button, file will be download.
   Currently no option to do mulitple upload.
9) You can delete you account from Personal Data under Manage user accounts.
10) Password can be updated from Password under Manage accounts. Kindly note, as there is no email functionality currently hence user can only change the password from Manage Accounts.
11) Kindly leave Email and Two FactoreAuthentication.
12) Visual studio 2019
13) SQL SERVER 18.9.1
14) Dot New Framework 5.0
   
      
