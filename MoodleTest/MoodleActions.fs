﻿namespace MoodleTest

open System
open System.IO
open OpenQA.Selenium
open OpenQA.Selenium.Remote
open OpenQA.Selenium.Chrome
open NModel.Terms
open MoodleModel

type Moodle (?arg : String) =
    let MOODLE_SITE = "http://localhost:8081/moodle/login/index.php"
    let COOKIE_SITE = "http://localhost:8081/moodle/xdebugcookie.php"
    let COURSE_ADMIN_SITE = "http://localhost:8081/moodle/user/index.php?contextid=28&id=2&perpage=200"
    let MOODLE_USERNAME_ADMIN = "admin"
    let MOODLE_PASSWORD_ADMIN = "05cT8eEe#"
    let MOODLE_PASSWORD_CORRECT = "AjutineParool1#"
    let MOODLE_PASSWORD_INCORRECT = "ValeParool1#"
    let MOODLE_SEARCH_VALID = "Testkursus 1"
    let MOODLE_SEARCH_INVALID = "Invalid"
    let MOODLE_ENROLMENT_KEY_VALID = "TK1"
    let MOODLE_ENROLMENT_KEY_INVALID = "Invalid"

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
    let courseActivity = "activity"
    let quizButton = "quizstartbuttondiv"


    let options = ChromeOptions()

    let param_moodle_username = 
        match arg with 
            | None -> "tudeng"
            | Some a -> a.ToLower().Trim() 

    let param_fullname = 
        match arg with 
            | None -> "Tudeng Tudeng"
            | Some a -> a.Trim() + " " + a.Trim()

    let log (text : string) =
        System.Console.Error.WriteLine(text)
     
    
    static member val driver = null with get, set

    member this.Init () =
        // set the chrome version to latest
        options.AddAdditionalCapability(CapabilityType.Version, "latest", true)
        // set the operating system
        options.AddAdditionalCapability(CapabilityType.Platform, "WIN8", true)
        // run chrome driver without graphical ui and disable gpu
        options.AddArgument("headless")
        options.AddArgument("disable-gpu")
        // path to chrome driver  
        Moodle.driver <- new ChromeDriver("MoodleTest" + string (System.IO.Path.DirectorySeparatorChar), options)
        //time to wait for elements to appear 
        Moodle.driver.Manage().Timeouts().ImplicitWait <- TimeSpan.FromSeconds(5.0)
        // check if argument given to adapter is "Tudeng" - if yes, then init XDebug
        if arg.IsSome then
            let argValue = Option.get arg
            if argValue = "Tudeng" then
                this.SetXDebugcookie ()
        // open moodle home page
        this.OpenMoodle() 

    member this.SavePageSource () =
        match Moodle.driver.Url.Contains(".php") with
            | true -> let pageSource = Moodle.driver.PageSource
                      File.WriteAllText ("Logs" + string (System.IO.Path.DirectorySeparatorChar) + "pagesource-" + System.DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".html", pageSource)
                      
            | _ -> ()                      

    member this.SetXDebugcookie () = 
        Moodle.driver.Navigate().GoToUrl(COOKIE_SITE)
        System.Threading.Thread.Sleep(2000)

    member this.OpenMoodle () =
        Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
        
    member this.Login (t:CompoundTerm) = 
        // Find the input fields and login button in web page
        let usernameField = Moodle.driver.FindElementById(username)
        let passwordField = Moodle.driver.FindElementById(password)
        let loginButton = Moodle.driver.FindElementById(loginButton)
        // Insert username
        usernameField.SendKeys(param_moodle_username)
        // Check whether the password to insert should be correct or incorrect
        if (string((t).[1]) = "Password(\"Correct\")") then 
            passwordField.SendKeys(MOODLE_PASSWORD_CORRECT)
        else 
            passwordField.SendKeys(MOODLE_PASSWORD_INCORRECT)        
        // Submit the input field values
        loginButton.Submit()
        // Validate login
        let visibleElementWhenLoginSuccess = Moodle.driver.FindElementsById(itemVisibleWhenLoginSuccess)
        // Create a compound term to return
        if visibleElementWhenLoginSuccess.Count > 0 then
            Action.Create("login_finish", (t).[0], LoginStatus.Success) :> CompoundTerm
        else 
            Action.Create("login_finish", (t).[0], LoginStatus.Failure) :> CompoundTerm

    member this.Search (t : CompoundTerm) = 
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

    member private this.LogoutHelper () =
        let userDropdown = Moodle.driver.FindElementById(settingsDropdown)
        userDropdown.Click()
        let logoutButton = Moodle.driver.FindElementById(logoutButton)
        logoutButton.Click()

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

        let visibleActivity = Moodle.driver.FindElementsByClassName(courseActivity)
        if visibleActivity.Count > 0 then 
            Action.Create("enrol_finish", (t).[0], EnrolmentStatus.Successful) :> CompoundTerm
        else            
            Action.Create("enrol_finish", (t).[0], EnrolmentStatus.Failed) :> CompoundTerm
           
    member this.Quiz () =
        let quizLink = Moodle.driver.FindElementByXPath("//div[@class='activityinstance']/a")
        quizLink.Click() 
        let quizButton = Moodle.driver.FindElementByClassName(quizButton)
        quizButton.Click()
        let question1 = Moodle.driver.FindElementByXPath("//div[@id='q1']//input[@value='2']")
        question1.Click()
        let question2 = Moodle.driver.FindElementByXPath("//div[@id='q2']//input[@value='2']")
        question2.Click()
        let question3 = Moodle.driver.FindElementByXPath("//div[@id='q3']//input[@value='2']")
        question3.Click()       
        let question4 = Moodle.driver.FindElementByXPath("//div[@id='q4']//input[@value='2']")
        question4.Click()
        let question5 = Moodle.driver.FindElementByXPath("//div[@id='q5']//input[@value='2']")
        question5.Click()
        System.Threading.Thread.Sleep(1000)
        let finishAttempt = Moodle.driver.FindElementByXPath("//div[@class='submitbtns']/input")
        finishAttempt.Click()
        System.Threading.Thread.Sleep(1000)
        let submitAll = Moodle.driver.FindElementByXPath("//div[@id='quiz-timer']/following::div//button")
        submitAll.Click()
        System.Threading.Thread.Sleep(1000)
        let confirmButton = Moodle.driver.FindElementByXPath("//div[contains(@class, 'confirmation-buttons')]/input[contains(@class,'btn-primary')]")
        confirmButton.Click()
        System.Threading.Thread.Sleep(1000)
        Action.Create("quiz_finish") :> CompoundTerm

        
        
    member private this.LoginAsAdmin () = 
        Moodle.driver.Navigate().GoToUrl(MOODLE_SITE)
        let usernameField = Moodle.driver.FindElementById(username)
        let passwordField = Moodle.driver.FindElementById(password)
        let loginButton = Moodle.driver.FindElementById(loginButton)
        usernameField.SendKeys(MOODLE_USERNAME_ADMIN)
        passwordField.SendKeys(MOODLE_PASSWORD_ADMIN)
        loginButton.Submit()
    
    member this.UnEnrol () = 
        // see if user is currently still logged in. If yes then log the user out
        let elementVisibleWhenUserLoggedIn = Moodle.driver.FindElementsById(itemVisibleWhenLoginSuccess)
        if elementVisibleWhenUserLoggedIn.Count > 0 then
            this.LogoutHelper ()       
        // Log in as admin 
        this.LoginAsAdmin ()
        // Go to course admin page
        Moodle.driver.Navigate().GoToUrl(COURSE_ADMIN_SITE)
        let unenrolUser = Moodle.driver.FindElementsByXPath("//div[@data-fullname='" + param_fullname + "']/a[@data-action='unenrol']")
        // Unenrol current user
        if unenrolUser.Count > 0 then
            unenrolUser.Item(0).Click()            
            System.Threading.Thread.Sleep(2000)
            let submit = Moodle.driver.FindElementByXPath("//div[@class='modal-content']/div[@class='modal-footer']/button[1]")
            submit.Click()
            System.Threading.Thread.Sleep(2000)
        ()



