namespace MoodleModel

open NModel
open NModel.Attributes
open NModel.Terms
open NModel.Execution

type MoodleMode = Initial = 0 | Running = 1
type View = Login = 0 | Dashboard = 1 
type User = Student1 = 0 
type Password = Correct = 0 | Incorrect = 1
type LoginStatus = Success = 0 | Failure = 1 

type Contract() = 
    static member val mode = MoodleMode.Initial with get, set 
    static member val view = View.Login with get, set
    static member val currentLogins = Map<User, LoginStatus>.EmptyMap with get, set

    [<Action>]
    static member initialize() = Contract.mode <- MoodleMode.Running
    static member initializeEnabled () = Contract.mode = MoodleMode.Initial

    [<Action>]
    static member loginstart (user, password) = 
        //match password with
        //| Password.Correct -> 
        //    Contract.view <- View.Dashboard
        //    Contract.currentLogins <- Contract.currentLogins.Add(user, LoginStatus.Success)
        //| _ -> Contract.currentLogins <- Contract.currentLogins.Add(user, LoginStatus.Failure)
        if password = Password.Correct then
            Contract.view <- View.Dashboard
            Contract.currentLogins <- Contract.currentLogins.Add(user, LoginStatus.Success)
        else
            Contract.currentLogins <- Contract.currentLogins.Add(user, LoginStatus.Failure)

    static member loginstartEnabled () =
        Contract.mode = MoodleMode.Running && 
        Contract.view = View.Login

    [<Action>]
    static member loginfinish(user : User, loginStatus : LoginStatus) = 
        ()
    static member loginfinishEnabled(user : User, loginStatus : LoginStatus) = 
        Contract.currentLogins.Contains(Pair<User, LoginStatus> (user, loginStatus))
    
    static member Create()  =
        LibraryModelProgram (typedefof<Contract>.Assembly, "MoodleModel")