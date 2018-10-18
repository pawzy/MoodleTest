namespace MoodleModel

open System
open NModel
open NModel.Attributes
open NModel.Execution
open NModel

type View = Login = 0 | DashboardAuthenicated = 1 | Dashboard = 2 | CourseSearch = 3 | CourseEnrol = 4 | CourseOverview = 5
type User = Student = 0 
type Password = Correct = 0 | Incorrect = 1
type LoginStatus = Success = 0 | Failure = 1 
type LogoutStatus = Success = 0 | Failure = 1
type GuestSession = Active = 0 | Inactive = 1
type SearchKey = ValidCourse = 0 | InvalidCourse = 1
type SearchStatus = Found = 0 | Notfound = 1
type EnrolmentKey = Correct = 0 | Incorrect = 1
type EnrolmentStatus = Successful = 0 | Failed = 1

type Contract() = 
    static member val view = View.Login with get, set

    // login & logout 
    static member val activeLoginRequests = Map<User,LoginStatus>.EmptyMap with get, set
    static member val usersLoggedIn : Set<User> = Set<User>.EmptySet with get,set
    static member val activeLogoutRequests : Set<User> = Set<User>.EmptySet with get, set
    // search
    static member val currentSearch = Map<SearchKey, SearchStatus>.EmptyMap with get, set
    static member val foundCourse = false with get, set
    // enrolment
    static member val activeEnrolRequests = Map<User, EnrolmentStatus>.EmptyMap with get, set
    static member val courseParticipants = Set<User>.EmptySet with get, set

    [<Action>]
    static member loginStart (user : User, password : Password) = 
        Contract.view <- View.DashboardAuthenicated
        if password = Password.Correct then
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Success)
        else
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Failure)
            //Contract.view <- View.Login
    static member loginStartEnabled () =
        Contract.view = View.Login && 
        Contract.usersLoggedIn.IsEmpty 


    [<Action>]
    static member loginfinish(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests <- Contract.activeLoginRequests.RemoveKey(user)

        if loginStatus = LoginStatus.Success then 
            Contract.usersLoggedIn <- Set<User> (user)
        else 
            Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
            Contract.view <- View.Login
    static member loginfinishEnabled(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests.Contains(Pair<User, LoginStatus> (user, loginStatus)) && 
        Contract.view = View.DashboardAuthenicated
    
    //static member Create()  =
    //    LibraryModelProgram (typedefof<Contract>.Assembly, "MoodleModel")

    [<Action>]
    static member logoutstart (user : User) = 
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Add(user)
        Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
        Contract.view <- View.Dashboard
    static member logoutstartEnabled (user : User) = 
        Contract.usersLoggedIn.Contains(user)  &&
        Contract.currentSearch.Count = 0


    [<Action>]
    static member logoutFinish (user : User) =
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Remove(user)
    static member logoutFinishEnabled (user : User) =
        Contract.view = View.Dashboard &&
        Contract.usersLoggedIn.Contains(user) = false &&
        Contract.activeLogoutRequests.Contains(user)
        
    [<Action>]
    static member searchStart (keyword : SearchKey) =
        Contract.view <- View.CourseSearch
        Contract.foundCourse <- false
        if keyword = SearchKey.ValidCourse then
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Found)
        else 
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Notfound)
    static member searchStartEnabled ()  =
        Contract.usersLoggedIn.Count > 0 &&
        Contract.currentSearch.Count = 0 &&
        (Contract.view = View.DashboardAuthenicated ||
         Contract.view = View.CourseSearch)
    
    [<Action>]
    static member searchFinish (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch <- Contract.currentSearch.RemoveKey(keyword)
        if status = SearchStatus.Found then
            Contract.foundCourse <- true
        else 
            Contract.foundCourse <- false
    static member searchFinishEnabled (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch.Contains(Pair<SearchKey, SearchStatus> (keyword, status)) &&
        Contract.view = View.CourseSearch

    [<Action>]
    static member enrolStart (user : User, enrolmentKey : EnrolmentKey) = 
        Contract.view <- View.CourseEnrol
        if enrolmentKey = EnrolmentKey.Correct then 
            Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Successful)
        else 
        //    Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Failed)
            Contract.activeEnrolRequests
    static member enrolStartEnabled (user : User) =
        Contract.usersLoggedIn.Contains(user) &&
        Contract.foundCourse = true &&
        Contract.view = View.CourseSearch &&
        Contract.courseParticipants.Contains(user) = false

    //[<Action>]
    //static member enrolFinish (user : User, enrolmentStatus : EnrolmentStatus) =
    //    Contract.activeEnrolRequests <- Contract.activeEnrolRequests.Remove(Pair<User, EnrolmentStatus> (user, enrolmentStatus))
    //    if enrolmentStatus = EnrolmentStatus.Successful then 
    //        Contract.view <- View.CourseOverview 
    //        Contract.courseParticipants <- Contract.courseParticipants.Add(user)
    //    else 
    //        Contract.view <- View.CourseEnrol
    //        Contract.courseParticipants <- Contract.courseParticipants.Remove(user)
    //static member enrolFinishEnabled (user : User, enrolmentStatus : EnrolmentStatus) = 
    //    Contract.activeEnrolRequests.Contains(Pair<User, EnrolmentStatus> (user, enrolmentStatus)) &&
    //    Contract.view = View.CourseEnrol
    
    //[<Action>]
    //static member quizStart (user : User) = 
    //    ()
    //static member quizStartEnabled (user : User) =
    //    Contract.courseParticipants.Contains(user)


type Factory() =
    static member Create() =
        LibraryModelProgram (typedefof<Factory>.Assembly, "MoodleModel")