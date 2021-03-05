	// Controller Seat should contain this
        string seatName = "[Ref]";

        // Name of your groups for left track
        string motorGroupLeftA = "Motors - Left - Track A";     //Track A - Left Rotors
        string motorGroupRightA = "Motors - Right - Track A";   //Track A - Right Rotors
        
        // Name of your groups for right track
        string motorGroupLeftB = "Motors - Left - Track B";     //Track B - Left Rotors
        string motorGroupRightB = "Motors - Right - Track B";   //Track B - Right Rotors

        // Tracks can be inverted
        bool invertA = false; //Left
        bool invertB = false; //Right

        // Speed for rotors
        float RPM = 60;

        //------------------END OF SETTINGS-----------------//
        // No touchy from here!
        //------------------END OF SETTINGS-----------------//

        List<IMyMotorStator> leftMotorsA = new List<IMyMotorStator>();
        List<IMyMotorStator> rightMotorsA = new List<IMyMotorStator>();

        List<IMyMotorStator> leftMotorsB = new List<IMyMotorStator>();
        List<IMyMotorStator> rightMotorsB = new List<IMyMotorStator>();

        List<IMyShipController> controlles = new List<IMyShipController>();
        Vector3 moveIndicator = new Vector3();

        IMyBlockGroup leftA, leftB, rightA, rightB;

        readonly string[] runStatus = new[]
        {
        "Running [|-----]",
        "Running [-|----]",
        "Running [--|---]",
        "Running [---|--]",
        "Running [----|-]",
        "Running [-----|]",
        "Running [----|-]",
        "Running [---|--]",
        "Running [--|---]",
        "Running [-|----]"
        };
        int runAdvancer;
        string dateVersion = "Version: 1.00 05/03/21";

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            GridTerminalSystem.GetBlocksOfType(controlles, b => b.IsSameConstructAs(Me) && b.IsFunctional && b.CustomName.Contains(seatName));

           
            leftA = GridTerminalSystem.GetBlockGroupWithName(motorGroupLeftA);
            rightA = GridTerminalSystem.GetBlockGroupWithName(motorGroupRightA);

            leftB = GridTerminalSystem.GetBlockGroupWithName(motorGroupLeftB);
            rightB = GridTerminalSystem.GetBlockGroupWithName(motorGroupRightB);

            if (leftA != null)
                leftA.GetBlocksOfType(leftMotorsA);
            if (rightA != null)
                rightA.GetBlocksOfType(rightMotorsA);

            if (leftB != null)
                leftB.GetBlocksOfType(leftMotorsB);
            if (rightB != null)
                rightB.GetBlocksOfType(rightMotorsB);

            runAdvancer = (runAdvancer + 1) % runStatus.Length;
            Echo("Dragon's Sub-Grid Track Controller\n" + dateVersion + "\n" + runStatus[runAdvancer] + "\n");

            if (controlles.Count() >= 1)
            {
                Echo("Reference: " + controlles[0].CustomName);
                if (controlles[0].IsUnderControl)
                {
                    moveIndicator = controlles[0].MoveIndicator;
                }
                else
                {
                    moveIndicator = Vector3.Zero;
                }
            }
            else
            {
                Echo(">> ERROR:\nNo seat with custom name containing " + seatName + " found.");
            }

            if (moveIndicator.Z == 1 || moveIndicator.Z == -1)
            {
                ControlTrack(leftMotorsA, rightMotorsA, invertA, RPM, moveIndicator.Z);
                ControlTrack(leftMotorsB, rightMotorsB, !invertB, RPM, moveIndicator.Z);
            }
            if (moveIndicator.X == 1 || moveIndicator.X == -1)
            {
                ControlTrack(leftMotorsA, rightMotorsA, !invertA, RPM, moveIndicator.X);
                ControlTrack(leftMotorsB, rightMotorsB, !invertB, RPM, moveIndicator.X);
            }
            if (moveIndicator.X == 0 && moveIndicator.Z == 0)
            {
                ControlTrack(leftMotorsA, rightMotorsA, invertA, 0, 0);
                ControlTrack(leftMotorsB, rightMotorsB, invertB, 0, 0);
            }
        }

        void ControlTrack(List<IMyMotorStator> left, List<IMyMotorStator> right, bool invert, float RPM, float moveNum)
        {
            if(moveNum == -1 && !invert)
            {
                SetRPM(left, RPM);
                SetRPM(right, -RPM);
            }
            else if(moveNum == -1 && invert)
            {
                SetRPM(left, -RPM);
                SetRPM(right, RPM);
            }
            else
            {
                if (moveNum == 1 && !invert)
                {
                    SetRPM(left, -RPM);
                    SetRPM(right, RPM);
                }
                else if (moveNum == 1 && invert)
                {
                    SetRPM(left, RPM);
                    SetRPM(right, -RPM);
                }
                else
                {
                    SetRPM(left, 0);
                    SetRPM(right, 0);
                }
            }       
        }

        void SetRPM(List<IMyMotorStator> rotors, float RPM)
        {
            foreach(var block in rotors)
            {
                block.TargetVelocityRPM = RPM;
            }
        }
