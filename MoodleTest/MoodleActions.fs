namespace MoodleTest

open System
open OpenQA.Selenium
open OpenQA.Selenium.Remote
open OpenQA.Selenium.Chrome
open NModel
open NModel.Conformance
open NModel.Terms

open MoodleModel

type Moodle () =
    let MOODLE_SITE = "http://localhost:8081/moodle/login/index.php"
    let COOKIE_SITE = "http://localhost:8081/moodle/xdebugcookie.php"
    let MOODLE_USERNAME = "tudeng"
    let MOODLE_PASSWORD_CORRECT = "AjutineParool1#"
    let MOODLE_PASSWORD_INCORRECT = "ValeParool1#"
    let MOODLE_SEARCH_VALID = "Testkursus 1"
    let MOODLE_SEARCH_INVALID = "Invalid"
    let MOODLE_ENROLMENT_KEY_VALID = "TK1"
    let MOODLE_ENROLMENT_KEY_INVALID = "Invalid"

    //let MOODLE_PASSWORD_INCORRECT = "pw"
    let username = "username"
    let password = "password"
    let loginButton = "loginbtn"
    let itemVisibleWhenLoginSuccess = "nav-notification-popover-container"
    let searchBoxMain = "coursesearchbox"
    let searchBoxDashboard = "search-box"
    let settingsDropdown = "dropdown-1"
    let logoutButton = "actionmenuaction-5"
    let enrolmentKey = "enrolpassword_4"
    let courseBox = "coursebox"
    let selectCourse = "//div[contains(@class,'coursebox')]/div/h3/a"
    let enrolButton = "id_submitbutton"
    let courseHeader = "course-header"

    let options = ChromeOptions()
    
    static member val driver = null with get, set

    member this.Init () =
        options.AddAdditionalCapability(CapabilityType.Version, "latest", true)
        options.AddAdditionalCapability(CapabilityType.Platform, "WIN8", true);
        options.AddAdditionalCapability("key", "key", true);
        options.AddAdditionalCapability("secret", "secret", true);
        options.AddAdditionalCapability("name", "katsetus", true);
        Moodle.driver <- new ChromeDriver(options)
        this.OpenMoodle() 
    
    member this.SetXDebugcookie () = 
        Moodle.driver.Navigate().GoToUrl(COOKIE_SITE)

    member this.OpenMoodle () =
        Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
        
    member this.Login (t:CompoundTerm) = 
        let usernameField = Moodle.driver.FindElementById(username)
        let passwordField = Moodle.driver.FindElementById(password)
        let loginButton = Moodle.driver.FindElementById(loginButton)
   
        usernameField.SendKeys(MOODLE_USERNAME)

        if (string((t).[1]) = "Password(\"Correct\")") then 
            passwordField.SendKeys(MOODLE_PASSWORD_CORRECT)
        else 
            passwordField.SendKeys(MOODLE_PASSWORD_INCORRECT)        
        
        loginButton.Submit()
        let visibleElementWhenLoginSuccess = Moodle.driver.FindElementsById(itemVisibleWhenLoginSuccess)

        //let visibleElementWhenLoginSuccess = 
        //    match Moodle.driver.FindElementById(itemVisibleWhenLoginSuccess) with 
        //    | null -> false
        //    | _ -> true
            
        if visibleElementWhenLoginSuccess.Count > 0 then
            Action.Create("login_finish", (t).[0], LoginStatus.Success) :> CompoundTerm
        else 
            Action.Create("login_finish", (t).[0], LoginStatus.Failure) :> CompoundTerm

    member this.Search (t : CompoundTerm) = 
        //let searchBox = null 
        let searchBox = 
            match Moodle.driver.Url.Contains("search.php") with
            | true -> Moodle.driver.FindElementById(searchBoxMain)
            | _ -> Moodle.driver.FindElementById(searchBoxDashboard)
        
        searchBox.Clear()
        if (string((t).[0]) = "SearchKey(\"ValidCourse\")") then 
            searchBox.SendKeys(MOODLE_SEARCH_VALID)
        else 
            searchBox.SendKeys(MOODLE_SEARCH_INVALID)
        
        searchBox.SendKeys(Keys.Enter)

        let results = Moodle.driver.FindElementsByClassName(courseBox)
        if results.Count = 0 then 
            Action.Create("search_finish", (t).[0], SearchStatus.Notfound) :> CompoundTerm
        else 
            Action.Create("search_finish", (t).[0], SearchStatus.Found) :> CompoundTerm

    member this.Logout (t : CompoundTerm) =
        let userDropdown = Moodle.driver.FindElementById(settingsDropdown)
        
        userDropdown.Click()
        let logoutButton = Moodle.driver.FindElementById(logoutButton)
        logoutButton.Click()
        Action.Create("logout_finish", (t).[0]) :> CompoundTerm

    member this.Enrol (t : CompoundTerm) = 
        let course = Moodle.driver.FindElementByXPath(selectCourse)
        course.Click()
        let enrolmentKey = Moodle.driver.FindElementById(enrolmentKey)
        if (string((t).[1]) = "EnrolmentKey(\"Correct\")") then
            enrolmentKey.SendKeys(MOODLE_ENROLMENT_KEY_VALID)
        else 
            enrolmentKey.SendKeys(MOODLE_ENROLMENT_KEY_INVALID)
        let enrolButton = Moodle.driver.FindElementById(enrolButton)
        enrolButton.Submit()

        let courseHeaderText = Moodle.driver.FindElementsById(courseHeader)
        if courseHeaderText.Count > 0 then 
            this.UnEnrol ()

            Action.Create("enrol_finish", (t).[0], EnrolmentStatus.Successful) :> CompoundTerm
        else
            //this.UnEnrol ()
            
            Action.Create("enrol_finish", (t).[0], EnrolmentStatus.Failed) :> CompoundTerm
           


    member this.UnEnrol () = 
        Moodle.driver.Navigate().GoToUrl("http://localhost:8081/moodle/course/view.php?id=2")
        let dropdown = Moodle.driver.FindElementById("dropdown-2")
        dropdown.Click()
        let unenrolButton = Moodle.driver.FindElementById("action-menu-2-menu")
        unenrolButton.Click()
        let confirm = Moodle.driver.FindElementsByClassName("singlebutton").Item(0)
        confirm.Click()

        ()




    //static member public OpenBrowser () : ChromeDriver = 
    //    ()
   

    
    //static member public CorrectLogin (driver : ChromeDriver) =
    //    if driver = null then null
    //    else driver.Navigate().GoToUrl(MOODLE_SITe) driver
            

    //interface IStepper with 
    //    member this.DoAction(t:CompoundTerm) = 
    //        match t.FunctionSymbol.ToString() with 
    //        | "initialize" -> 
    //            Moodle.driver <- Moodle.OpenBrowser() 
    //            //term v6imaldab tagasi saata andmeid mudelisse
    //            null
    //        | "login" -> 
    //            Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
    //            Moodle.driver.FindElementByLinkText("Log in").Click()
    //            Moodle.driver.FindElementById("username").SendKeys("tudeng")
    //            Moodle.driver.FindElementById("password").SendKeys("AjutineParool1#")
    //            Moodle.driver.FindElementById("loginbtn").Submit()
    //            //Moodle.Driver.FindElementById("x")
           
    //            null
    //        | s -> failwith (sprintf "Tundmatu toiming %s" s)
    //    member this.Reset() = 
    //        Moodle.driver.Dispose()
    //        Moodle.driver <- Moodle.OpenBrowser() 
    //        ()
    

    //member this.CorrectLogin () =
    //    Moodle.driver.Navigate.GoToUrl(Moodle_SITE)


    //static member Create () = 
    //    new Moodle() 


        //let driver = Moodle.OpenBrowser



        //driver.Navigate().GoToUrl("http://localhost:8081/moodle/")
        //driver.FindElementByLinkText("Log in").Click()
        //driver.FindElementById("username").SendKeys("tudeng")
        //driver.FindElementById("password").SendKeys("AjutineParool1#")
        //driver.FindElementById("loginbtn").Submit()

        //driver



