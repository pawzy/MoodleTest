namespace MoodleModel

open System
open NModel
open NModel.Attributes
open NModel.Execution
open NModel

type View = Login = 0 | DashboardAuthenicated = 1 | Dashboard = 2 | CourseSearch = 3
type User = StudentA = 0 
type Password = Correct = 0 | Incorrect = 1
type LoginStatus = Success = 0 | Failure = 1 
type LogoutStatus = Success = 0 | Failure = 1
type GuestSession = Active = 0 | Inactive = 1
type SearchKey = ValidCourse = 0 | InvalidCourse = 1
type SearchStatus = Found = 0 | Notfound = 1

type Contract() = 
    static member val view = View.Login with get, set

    // login & logout 
    static member val activeLoginRequests = Map<User,LoginStatus>.EmptyMap with get, set
    static member val usersLoggedIn : Set<User> = Set<User>.EmptySet with get,set
    static member val activeLogoutRequest : Set<User> = Set<User>.EmptySet with get, set
    // search
    static member val currentSearch = Map<SearchKey, SearchStatus>.EmptyMap with get, set

    [<Action>]
    static member loginstart (user : User, password : Password) = 
        Contract.view <- View.DashboardAuthenicated
        if password = Password.Correct then
            Console.WriteLine(Contract.activeLoginRequests.ToString)
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Success)
        else
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Failure)
            //Contract.view <- View.Login
    static member loginstartEnabled () =
        Contract.view = View.Login && 
        Contract.usersLoggedIn.IsEmpty 


    [<Action>]
    static member loginfinish(user : User, loginStatus : LoginStatus) = 
        if loginStatus = LoginStatus.Success then 
            Contract.usersLoggedIn <- Set<User> (user)
        else 
            Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
        Contract.activeLoginRequests <- Contract.activeLoginRequests.RemoveKey(user)
    static member loginfinishEnabled(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests.Contains(Pair<User, LoginStatus> (user, loginStatus)) && 
        Contract.view = View.DashboardAuthenicated
    
    //static member Create()  =
    //    LibraryModelProgram (typedefof<Contract>.Assembly, "MoodleModel")

    [<Action>]
    static member logoutstart (user : User) = 
        Contract.activeLogoutRequest <- Contract.activeLogoutRequest.Add(user)
        Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
        Contract.view <- View.Dashboard
    static member logoutstartEnabled (user : User) = 
        Contract.usersLoggedIn.Contains(user)  &&
        Contract.currentSearch.Count = 0


    [<Action>]
    static member logoutFinish (user : User) =
        Contract.activeLogoutRequest <- Contract.activeLogoutRequest.Remove(user)
    static member logoutFinishEnabled (user : User) =
        Contract.view = View.Dashboard &&
        Contract.usersLoggedIn.Contains(user) = false &&
        Contract.activeLogoutRequest.Contains(user)
        
    [<Action>]
    static member searchStart (keyword : SearchKey) =
        Contract.view <- View.CourseSearch
        if keyword = SearchKey.ValidCourse then
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Found)
        else 
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Notfound)
    static member searchStartEnabled ()  =
        Contract.usersLoggedIn.Count > 0 &&
        Contract.currentSearch.Count = 0
    
    [<Action>]
    static member searchFinish (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch <- Contract.currentSearch.RemoveKey(keyword)
    static member searchFinishEnabled (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch.Contains(Pair<SearchKey, SearchStatus> (keyword, status)) &&
        Contract.view = View.CourseSearch
type Factory() =
    static member Create() =
        LibraryModelProgram (typedefof<Factory>.Assembly, "MoodleModel")