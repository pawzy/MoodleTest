namespace MoodleModel

open System
open NModel
open NModel.Attributes
open NModel.Execution
open NModel

type View = Login = 0 | DashboardAuthenicated = 1 | Dashboard = 2 | CourseSearch = 3 | CourseEnrol = 4 | CourseOverview = 5 | Quiz = 6 | QuizResults = 7
type User = Student = 0 
type Password = Correct = 0 | Incorrect = 1
type public LoginStatus = Success = 0 | Failure = 1 
type LogoutStatus = Success = 0 | Failure = 1
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
    static member val enrolStarted = false with get, set
    // quiz
    static member val quizStarted = false with get, set

    [<Action>]
    static member login_start (user : User, password : Password) = 
        Contract.view <- View.DashboardAuthenicated
        if password = Password.Correct then
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Success)
        else
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Failure)
            //Contract.view <- View.Login
    static member login_startEnabled (user : User) =
        Contract.view = View.Login && 
        Contract.usersLoggedIn.Contains(user) = false 


    [<Action>]
    static member login_finish(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests <- Contract.activeLoginRequests.RemoveKey(user)

        if loginStatus = LoginStatus.Success then 
            Contract.usersLoggedIn <- Contract.usersLoggedIn.Add(user)
        else 
            //Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
            Contract.view <- View.Login
    static member login_finishEnabled(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests.Contains(Pair<User, LoginStatus> (user, loginStatus)) && 
        Contract.view = View.DashboardAuthenicated

    
    //static member logout (user: User) = 
    //    //()
    //    Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
    //    Contract.view <- View.Dashboard
    //static member logoutEnabled (user: User) =
    //    Contract.usersLoggedIn.Contains(user) && 
    //    Contract.currentSearch.Count = 0 &&
    //    Contract.activeEnrolRequests.Count = 0
    [<Action>]
    static member logout_start (user : User) = 
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Add(user)
        Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
        Contract.view <- View.Dashboard
    static member logout_startEnabled (user : User) = 
        Contract.usersLoggedIn.Contains(user)  &&
        Contract.currentSearch.Count = 0 &&
        Contract.activeEnrolRequests.Count = 0 


    [<Action>]
    static member logout_finish (user : User) =
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Remove(user)
    static member logout_finishEnabled (user : User) =
        Contract.view = View.Dashboard &&
        Contract.usersLoggedIn.Contains(user) = false &&
        Contract.activeLogoutRequests.Contains(user)
        

    [<Action>]
    static member search_start (keyword : SearchKey) =
        Contract.view <- View.CourseSearch
        Contract.foundCourse <- false
        if keyword = SearchKey.ValidCourse then
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Found)
        else 
            Contract.currentSearch <- Contract.currentSearch.Add(keyword, SearchStatus.Notfound)
    static member search_startEnabled ()  =
        Contract.usersLoggedIn.Count > 0 &&
        Contract.currentSearch.Count = 0 &&
        (Contract.view = View.DashboardAuthenicated ||
         Contract.view = View.CourseSearch)
    
    [<Action>]
    static member search_finish (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch <- Contract.currentSearch.RemoveKey(keyword)
        if status = SearchStatus.Found then
            Contract.foundCourse <- true
        else 
            Contract.foundCourse <- false
    static member search_finishEnabled (keyword : SearchKey, status : SearchStatus) =
        Contract.currentSearch.Contains(Pair<SearchKey, SearchStatus> (keyword, status)) &&
        Contract.view = View.CourseSearch

    [<Action>]
    static member enrol_start (user : User, enrolmentKey : EnrolmentKey) = 
        Contract.view <- View.CourseEnrol
        //Contract.enrolStarted <- true
        if enrolmentKey = EnrolmentKey.Correct then 
            Contract.activeEnrolRequests <- Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Successful)
        else 
            Contract.activeEnrolRequests <- Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Failed)
        //()
        
    static member enrol_startEnabled (user : User) =
        Contract.usersLoggedIn.Contains(user) &&
        Contract.foundCourse = true &&
        (Contract.view = View.CourseSearch || Contract.view = View.CourseEnrol) &&
        Contract.courseParticipants.Contains(user) = false  &&
        Contract.enrolStarted = false &&
        Contract.activeEnrolRequests.ContainsKey(user) = false
        

    [<Action>]
    static member enrol_finish (user : User, enrolmentStatus : EnrolmentStatus) =
        Contract.activeEnrolRequests <- Contract.activeEnrolRequests.RemoveKey(user)
        if enrolmentStatus = EnrolmentStatus.Successful then 
            Contract.view <- View.CourseOverview 
            Contract.courseParticipants <- Contract.courseParticipants.Add(user)
        else 
            Contract.view <- View.CourseEnrol
            Contract.courseParticipants <- Contract.courseParticipants.Remove(user)
    static member enrol_finishEnabled (user : User, enrolmentStatus : EnrolmentStatus) = 
        Contract.activeEnrolRequests.Contains(Pair<User, EnrolmentStatus> (user, enrolmentStatus)) &&
        Contract.view = View.CourseEnrol 
        
    [<Action>]
    static member quiz_start (user : User) = 
        Contract.view <- View.Quiz
        Contract.quizStarted <- true
    static member quiz_startEnabled (user : User) =
        Contract.courseParticipants.Contains(user) &&
        Contract.view = View.CourseOverview &&
        Contract.quizStarted = false
    [<Action>]
    static member quiz_finish () = 
        Contract.view <- View.QuizResults
        Contract.quizStarted <- false
    static member quiz_finishEnabled () =
        Contract.view = View.Quiz &&
        Contract.quizStarted = true


type Factory() =
    static member Create() =
        LibraryModelProgram (typedefof<Factory>.Assembly, "MoodleModel")