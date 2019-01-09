#if INTERACTIVE
#r @"bin\Debug\NModel.dll"
#r @"bin\Debug\NModel.Visualization.dll"
#r @"bin\Debug\MoodleModel.dll"
#endif

open NModel.Execution
open NModel.Terms
open NModel

let mp = MoodleModel.Factory.Create() 
NModel.Visualization.Interactive.Run(mp)


let fsm = FSM.FromString("FSM(0, AcceptingStates(), 
                    Transitions(t(0, login_start(User(\"Student\"), Password(\"Correct\")),1), 
                                t(1, login_finish(User(\"Student\"), LoginStatus(\"Success\")),3),
                                t(0, login_start(User(\"Student\"), Password(\"Incorrect\")), 2),
                                t(2, login_finish(User(\"Student\"), LoginStatus(\"Failure\")), 0)),
                    Vocabulary(\"login_start\", \"login_finish\"))")
let fsmmp = FsmModelProgram(fsm,"SomeScenario")
NModel.Visualization.Interactive.Run(fsmmp)

let fsm2 = FSM.FromString("FSM(0, AcceptingStates(), 
                    Transitions(t(0, login_start(),1), 
                                t(1, login_finish(), 2)),
                    Vocabulary(\"login_start\", \"login_finish\"))")
let fsmmp2 = FsmModelProgram(fsm2,"SomeScenario")
NModel.Visualization.Interactive.Run(fsmmp2)