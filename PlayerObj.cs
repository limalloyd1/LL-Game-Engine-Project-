using System;

namespace PlayerNameSpace
{
    public class PlayerClass
    {
        private float speed = 0.0f;
        private int color;
        private float direction;

        private static int NbPlayers;

        public int getNbPlayers()
        {
            return NbPlayers;
        }

        public float setColor()
        {
            return color;
        }

        public float setDirection(float newDirection)
        {
            // set while loop with keycode recognition
            direction = newDirection;
            return direction;
        }

        public float getDirection()
        {
            return direction;
        }

        public float increaseDirection(float currentDirection)
        {
            float newDirection = currentDirection + 1;
            return newDirection;
        }

        public float Player()
        {
            Console.WriteLine("Adding a player.");
            increaseNbPlayers();
            Console.WriteLine("Current players: " + NbPlayers);
            return NbPlayers;
        }
        public float getSpeed(float playerSpeed)
        {
            Console.WriteLine(playerSpeed);
            return playerSpeed;
        } 
        public float accelerate(float currentSpeed)
        {
            while (true)
            {
                currentSpeed = currentSpeed + 1;
                setSpeed(currentSpeed);
                getSpeed(currentSpeed);
                Console.WriteLine("Current speed: " + currentSpeed);
                Console.WriteLine("Enter 'x' to stop accelerating");
                
                // add keycode recognition
                string usrInput = Console.ReadLine();
                if (usrInput.ToLower() == "x")
                {
                    Console.WriteLine("Final Speed: " + currentSpeed);
                    return currentSpeed;
                }
            }
                
        }

        public static float increaseNbPlayers()
        {
            NbPlayers++;
            return NbPlayers;
        }

        public float setSpeed(float newSpeed)
        {
            speed = newSpeed;
            return newSpeed;
        }

        public void controllerInput()
        {
            
        }

        public float turnRight()
        {
            //TODO: find current playerSpeed
            float playerSpeed = speed;
            getSpeed(playerSpeed);

            float newSpeed = playerSpeed - 1;
            setSpeed(newSpeed);
            return newSpeed;
        }

        public float turnLeft()
        {
            //TODO: find current playerSpeed
            float newSpeed = 0.5f;
            setSpeed(newSpeed);
            return newSpeed;
            
        }

        public void jumpUp()
        {
            //setSpeed();
        }
        
        public void crouchDown()
        {
            //setSpeed();
        }
    }
}
