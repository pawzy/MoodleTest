namespace MoodleModel

open NModel
open NModel.Attributes
open NModel.Execution

type View = Login = 0 | DashboardAuthenicated = 1 | Dashboard = 2 | CourseSearch = 3 | CourseEnrol = 4 | CourseOverview = 5 | Quiz = 6 | QuizResults = 7
type User = Student = 0 
type Password = Correct = 0 | Incorrect = 1
type LoginStatus = Success = 0 | Failure = 1 
type LogoutStatus = Success = 0 | Failure = 1
type SearchKey = ValidCourse = 0 | InvalidCourse = 1
type SearchStatus = Found = 0 | Notfound = 1
type EnrolmentKey = Correct = 0 | Incorrect = 1
type EnrolmentStatus = Successful = 0 | Failed = 1

type RequirementAttribute = 
    class 
        [<DefaultValue>] val Id : string;
        [<DefaultValue>] val Summary : string;
        new() = {}
    end

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

    [<Requirement(Id = "Login start", Summary="The starting action for login. Takes two arguments: user (username) and password (correct or incorrect). 
                                                The action is enabled when the current view is login screen and the current user is not logged in.")>]
    [<Action>]
    static member login_start (user : User, password : Password) = 
        Contract.view <- View.DashboardAuthenicated
        if password = Password.Correct then
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Success)
        else
            Contract.activeLoginRequests <- Contract.activeLoginRequests.Add (user,LoginStatus.Failure)
    static member login_startEnabled (user : User) =
        Contract.view = View.Login && 
        Contract.usersLoggedIn.Contains(user) = false 

    [<Requirement(Id = "Login finish", Summary="The finish action for login. Takes two arguments: user (username) and loginStatus (success or failure).
                                                The action is enabled when login request has been started for the user and the view is authenticated
                                                user dashboard")>]
    [<Action>]
    static member login_finish(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests <- Contract.activeLoginRequests.RemoveKey(user)
        if loginStatus = LoginStatus.Success then 
            Contract.usersLoggedIn <- Contract.usersLoggedIn.Add(user)
        else 
            Contract.view <- View.Login
    static member login_finishEnabled(user : User, loginStatus : LoginStatus) = 
        Contract.activeLoginRequests.Contains(Pair<User, LoginStatus> (user, loginStatus)) && 
        Contract.view = View.DashboardAuthenicated

    [<Requirement(Id = "Logout start", Summary="The starting action for logout. Takes one argument: user (username). 
                                                The action is enabled when the user is currently logged in and there are no ongoing other actions")>]    
    [<Action>]
    static member logout_start (user : User) = 
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Add(user)
        Contract.usersLoggedIn <- Contract.usersLoggedIn.Remove(user)
        Contract.view <- View.Dashboard
    static member logout_startEnabled (user : User) = 
        Contract.usersLoggedIn.Contains(user)  &&
        Contract.currentSearch.Count = 0 &&
        Contract.activeEnrolRequests.Count = 0 &&
        Contract.quizStarted = false

    [<Requirement(Id = "Logout finish", Summary="The finish action for logout. Takes one arguments: user (username). 
                                                The action is enabled when the current view is dashboard, the user is not logged in 
                                                and there is a logout request for the specified user")>]
    [<Action>]
    static member logout_finish (user : User) =
        Contract.activeLogoutRequests <- Contract.activeLogoutRequests.Remove(user)
    static member logout_finishEnabled (user : User) =
        Contract.view = View.Dashboard &&
        Contract.usersLoggedIn.Contains(user) = false &&
        Contract.activeLogoutRequests.Contains(user)
        
    [<Requirement(Id = "Search start", Summary="The starting action for search. Takes on argument: keyword (valid or invalid).
                                                The action is enabled when user is logged in, there are no ongoing searches and the view 
                                                is either home page or course search page.")>]
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
 
    [<Requirement(Id = "Search finish", Summary="The finish action for search. Takes two arguments: keyword (valid or invalid) and status (found or not found).
                                                The action is enabled when the view is course search view and search has been started.")>]   
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
    
    [<Requirement(Id = "Enrol start", Summary="The start action for enrol. Takes two arguments: user (username) and enrolmentKey (corract or incorrect).
                                               The action is enabled when the user is logged in, course search was successful and the user is not enroled to the course.")>]   
    [<Action>]
    static member enrol_start (user : User, enrolmentKey : EnrolmentKey) = 
        Contract.view <- View.CourseEnrol
        if enrolmentKey = EnrolmentKey.Correct then 
            Contract.activeEnrolRequests <- Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Successful)
        else 
            Contract.activeEnrolRequests <- Contract.activeEnrolRequests.Add(user, EnrolmentStatus.Failed)

    static member enrol_startEnabled (user : User) =
        Contract.usersLoggedIn.Contains(user) &&
        Contract.foundCourse = true &&
        (Contract.view = View.CourseSearch || Contract.view = View.CourseEnrol) &&
        Contract.courseParticipants.Contains(user) = false  &&
        Contract.enrolStarted = false &&
        Contract.activeEnrolRequests.ContainsKey(user) = false
        
    [<Requirement(Id = "Enrol finish", Summary="The finish action for enrol. Takes two arguments: user (username) and enrolmentStatus (success or failure).
                                                The action is enabled when enrol request has been started and the current view is enrolment view.")>]   
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
    
    [<Requirement(Id = "Quiz start", Summary="The start action for quiz. Takes one argument: user (username). 
                                             The action is enabled when the user is enrolled to the course, the view is course dashboard and quiz has not been started.")>]   
    [<Action>]
    static member quiz_start (user : User) = 
        Contract.view <- View.Quiz
        Contract.quizStarted <- true
    static member quiz_startEnabled (user : User) =
        Contract.courseParticipants.Contains(user) &&
        Contract.view = View.CourseOverview &&
        Contract.quizStarted = false
    
    [<Requirement(Id = "Quiz finish", Summary="The start action for quiz. 
                                             The action is enabled when a quiz has been started and the current view is quiz view.")>]   
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