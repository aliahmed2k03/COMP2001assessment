# Ali Ahmed's COMP2001 Project

This is my COMP2001 Project where I have made a C# ASP.NET Web API that communicates with my database and allows users to access the stored procedures of my database through the API. 

I have three controllers:
1. Activities
2. Profiles
3. Users 

These controllers have their own operations as shown here:
![Swagger](C:\Users\marks\Pictures\Screenshot_1.png)

The Get operation on activities can be accessed anonymously and it returns all the activities that users can add to their profiles, this is useful for when the user is making a new profile and they don't know what activities there are to set as their FavouriteActivity so they can use this operation to see all available activities.

For the users controller, I didn't do any CRUD functions unless you count the endpoint register as a create operation, these two endpoints are just used to give the API proper authentication so that only authenticated individuals are allowed to use operations which write data to the database:

- the Register endpoint takes in three arguments from the user: username, email and password  and stores these values, the password is hashed and salted and then stored in the database for proper security
- the login endpoint takes in the same parameters and hashes and salts the password to compare it to the password in the database, these parameters are all compared and if they match then the user is succesfully authenticated, it also displays what the authenticator api response is to the email and password sent in.


For the profiles controller where I adapted CRUD functionality:
 
- The get endpoint returns all the profiles in the system and what user they are linked to with a limited view and what user owns them
- the post endpoint can only be used when logged in allows users to add profiles to the database
- the get{id} endpoint just gets the jsonObject data of a single profile
- the put{id} endpoint allows users to edit profiles that they own, they aren't allowed to edit anyone else's profile.
- The delete{id} endpoint just sets the given profiles PendingDeletion to 1 and lets the user know that their profile will be deleted by an admin soon.
- the archiveallprofiles is an operation only usable by admins and it archives every profile that has a pendingdeletion value of 1.

