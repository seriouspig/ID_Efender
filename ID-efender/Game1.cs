using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace ID_efender
{
    public class Game1 : Game
    {
        // Create the GameState enumerated type for 5 different states in the game
        enum GameState
        {
            Title,
            Instructions,
            Gameplay,
            HighScore,
            Gameover,
        }
        GameState _state;

        // Set the constants in the game: the number of walkers and spawn rates
        const int WALKERS = 100;
        const int BASESPAWNRATE = 4;
        const int PEBASESPAWNRATE = 20;

        // Create the graphics and spritebatch variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Create the random variable
        public readonly static Random RNG = new Random();        

        // Setting variables for spawn rates
        float spawnRate = BASESPAWNRATE, timeTillSpawn = BASESPAWNRATE;
        float pespawnRate = PEBASESPAWNRATE, petimeTillSpawn = PEBASESPAWNRATE;
        int enemiesPerSpawn = 1;
        int penemiesPerSpawn = 1;

        // Create variables for score and invasionmeter
        int invasionmeter = 0;
        int score = 0;              

        // Create variables for all the scrolling and still images
        Background background1_left, background1_right,
                   background2_left, background2_right,
                   background3_left, background3_right,
                   background4_left, background4_right,
                   background_sky, HUD, map, title, instructions, gameover, blackBackground;     
               
        // Create variables for current and previous keyboard and pad states
        KeyboardState keyb1_curr, keyb1_old;
        GamePadState pad1_curr, pad1_old;        

        // Create variables that are going to hold the fonts used for the text
        SpriteFont debug;
        SpriteFont scoretext;

        // Create variables that are going to be the lists of: bullets, expolsions, enemies and piper_enemies;
        List<Bullet> bullets;
        List<Explosion> explosions;
        List<Enemy> enemies;
        List<EnemyPiper> piperEnemies;

        // Create a variable that is going to be the array of walking people
        Walker[] walkers;

        // create variables for all the other game elements
        spaceship player;
        Explosion explosion;
        GoBack goback;
        PressStart pressStart;
        WarningSign warningSign;
        Piper piper;
        Invasionmeter meter;
        GameOverSign gameOverSign, newScore;

        // Create variables that are going to store the sound effects
        SoundEffect rocketEngine;
        SoundEffect piperloop;
        SoundEffect panic;
        SoundEffect shot;
        SoundEffect boom;
        SoundEffect select;
        SoundEffect gameOver;
        SoundEffect warning;

        // Create variables of instances of the sound effects
        SoundEffectInstance rocketEngineInstance;
        SoundEffectInstance piperloopInstance;
        SoundEffectInstance panicInstance;
        SoundEffectInstance shotInstance;
        SoundEffectInstance boomInstance;
        SoundEffectInstance selectInstance;
        SoundEffectInstance gameOverInstance;
        SoundEffectInstance warningInstance;        

        // Create variables for the text input
        InputBox testInput;
        string typedText;
        KeyboardState kb, oldkb;                          

        //Stuff for HighScoreData
        public struct HighScoreData
        {
            public string[] PlayerName;
            public int[] Score;
            public int Count;

            public HighScoreData(int count)
            {
                PlayerName = new string[count];
                Score = new int[count];

                Count = count;
            }
        }

        /* More score variables */
        HighScoreData data;
        public string HighScoresFilename = "highscores.dat";
        string scoreboard;        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 540;
            //set the GraphicsDeviceManager's fullscreen property
            // graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /* Save highscores */
        public static void SaveHighScores(HighScoreData data, string filename)
        {
            // Get the path of the save game
            string fullpath = "highscores.dat";

            // Open the file, creating it if necessary
            FileStream stream = File.Open(fullpath, FileMode.Create);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        /* Load highscores */
        public static HighScoreData LoadHighScores(string filename)
        {
            HighScoreData data;
            // Get the path of the save game
            string fullpath = "highscores.dat";

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }            
            return (data);
        }

        private void SaveHighScore()
        {
            // Create the data to save
            HighScoreData data = LoadHighScores(HighScoresFilename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {           
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.PlayerName[i] = data.PlayerName[i - 1];
                    data.Score[i] = data.Score[i - 1];
                }                

                data.PlayerName[scoreIndex] = typedText; //Retrieve User Name Here
                data.Score[scoreIndex] = score;

                SaveHighScores(data, HighScoresFilename);                           
            }
        }

        // Check if the new score is larger than the existing high score
        public bool newHSheck(HighScoreData data)
        {
            data = LoadHighScores(HighScoresFilename);
            bool foundHS = false;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Score[i])
                {
                    foundHS = true;
                    break;
                }
            }
            return foundHS;
        }

        /* Iterate through data if highscore is called and make the string to be saved*/
        public string makeHighScoreString()
        {
            // Create the data to save
            HighScoreData data2 = LoadHighScores(HighScoresFilename);

            // Create scoreBoardString
            string scoreBoardString = "HIGHSCORES:\n\n";

            for (int i = 0; i < 5; i++) // this part was missing (5 means how many in the list/array/Counter)
            {
                scoreBoardString = scoreBoardString + data2.PlayerName[i] + "-" + data2.Score[i] + "\n";
            }
            return scoreBoardString;
        }
          
        protected override void Initialize()
        {
            // initialization logic here
            // Create all new lists and arrays
            bullets = new List<Bullet>();
            enemies = new List<Enemy>();
            piperEnemies = new List<EnemyPiper>();
            walkers = new Walker[WALKERS];

            // Set all the sound instancess to null
            rocketEngineInstance = null;
            piperloopInstance = null;
            panicInstance = null;
            shotInstance = null;
            boomInstance = null;
            selectInstance = null;
            gameOverInstance = null;
            warningInstance = null;

            // Get the path of the save game
            string fullpath = "highscores.dat";

            // Check to see if the save exists
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                // Create the data to save
                data = new HighScoreData(5);
                data.PlayerName[0] = "pedro";
                data.Score[0] = 0;

                data.PlayerName[1] = "shawn";
                data.Score[1] = 0;

                data.PlayerName[2] = "mark";
                data.Score[2] = 0;

                data.PlayerName[3] = "cindy";
                data.Score[3] = 0;

                data.PlayerName[4] = "sam";
                data.Score[4] = 0;

                SaveHighScores(data, HighScoresFilename);
            }
            
            base.Initialize();                                         
                   
        }

        // Reset all the initial variables, positions, scores, etc.
        void resetGame()
        {
            // Clear all the lists
            bullets.Clear();
            enemies.Clear();
            piperEnemies.Clear();

            // reset the position of the walkers
            for (int i = 0; i < walkers.Length; i++)
                walkers[i] = new Walker(Content.Load<Texture2D>("walker_1"), Content.Load<Texture2D>("walker_map"), RNG, (int)background1_left.m_position.X, (int)background1_right.m_position.X + 2277, -1, 1, 4, 24, 0, 3);

            // reset all the numeric values to its original values
            invasionmeter = 0;
            score = 0;
            enemiesPerSpawn = 1;
            penemiesPerSpawn = 1;
            spawnRate = BASESPAWNRATE;
            pespawnRate = PEBASESPAWNRATE;        
            
            // reset the scrolling backgrounds to it´s original positions
            background1_left = new Background(Content.Load<Texture2D>("background1_left"), 0, 60);
            background1_right = new Background(Content.Load<Texture2D>("background1_right"), 2277, 60);
            background2_left = new Background(Content.Load<Texture2D>("background2_left"), -100, 60);
            background2_right = new Background(Content.Load<Texture2D>("background2_right"), 2177, 60);
            background3_left = new Background(Content.Load<Texture2D>("background3_left"), -200, 60);
            background3_right = new Background(Content.Load<Texture2D>("background3_right"), 2077, 60);
            background4_left = new Background(Content.Load<Texture2D>("background4_left"), -300, 40);
            background4_right = new Background(Content.Load<Texture2D>("background4_right"), 1977, 40);
            background_sky = new Background(Content.Load<Texture2D>("background_sky"), 0, 0);
            
            // reset the plane and piper to it´s original position
            player = new spaceship(Content.Load<Texture2D>("plane"), Content.Load<Texture2D>("spaceship_map"), graphics.PreferredBackBufferWidth / 2 - 200, graphics.PreferredBackBufferHeight / 2, 7, 1);
            piper = new Piper(Content.Load<Texture2D>("piper"), Content.Load<Texture2D>("piper_map"), 3000, 24);

            // set the warning sound instance to null
            warningInstance = null;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load and set the positions of all the scrolling and non scrolling background layers
            background1_left = new Background(Content.Load<Texture2D>("background1_left"), 0, 60);
            background1_right = new Background(Content.Load<Texture2D>("background1_right"), 2277, 60);
            background2_left = new Background(Content.Load<Texture2D>("background2_left"), -100, 60);
            background2_right = new Background(Content.Load<Texture2D>("background2_right"), 2177, 60);
            background3_left = new Background(Content.Load<Texture2D>("background3_left"), -200, 60);
            background3_right = new Background(Content.Load<Texture2D>("background3_right"), 2077, 60);
            background4_left = new Background(Content.Load<Texture2D>("background4_left"), -300, 40);
            background4_right = new Background(Content.Load<Texture2D>("background4_right"), 1977, 40);
            background_sky = new Background(Content.Load<Texture2D>("background_sky"), 0, 0);
            title = new Background(Content.Load<Texture2D>("title"), 0, 0);
            instructions = new Background(Content.Load<Texture2D>("instructions"), 0, 0);
            gameover = new Background(Content.Load<Texture2D>("gameover"), 0, 0);
            blackBackground = new Background(Content.Load<Texture2D>("black_background"), 716, 29);

            // Load and set the positions of all the other gameplay elements
            gameOverSign = new GameOverSign(Content.Load<Texture2D>("game_over"), 200, 200);
            newScore = new GameOverSign(Content.Load<Texture2D>("new_score"), 200, 400);
            goback = new GoBack(Content.Load<Texture2D>("go back"), 20, 200, 2, 24);
            pressStart = new PressStart(Content.Load<Texture2D>("press_start"), 380, 440, 24);
            warningSign = new WarningSign(Content.Load<Texture2D>("warning_sign"), 207, 77, 24);
            HUD = new Background(Content.Load<Texture2D>("HUD"), 0, 0);
            map = new Background(Content.Load<Texture2D>("map"), 250, 8);
            meter = new Invasionmeter(Content.Load<Texture2D>("invasionmeter"), 716, 29);
            player = new spaceship(Content.Load<Texture2D>("plane"), Content.Load<Texture2D>("spaceship_map"), graphics.PreferredBackBufferWidth / 2 - 200, graphics.PreferredBackBufferHeight / 2, 7, 1);
            piper = new Piper(Content.Load<Texture2D>("piper"), Content.Load<Texture2D>("piper_map"), 3000, 24);
            
            // Load the Sprite fonts
            debug = (Content.Load<SpriteFont>("debug"));
            scoretext = (Content.Load<SpriteFont>("score"));

            // Load bullet texture for the bullet list
            for (int i=0; i < bullets.Count; i++)
                Content.Load<Texture2D>("bullet");

            // Load the explosions for the explisions list
            explosion = new Explosion(Content.Load<Texture2D>("explosion"), 100, 100, 7, 24);
            explosions = new List<Explosion>();

            // Load the textures for the enemies list
            for (int i = 0; i < enemies.Count; i++)
            {
                Content.Load<Texture2D>("enemy");
                Content.Load<Texture2D>("enemy_map");
            }

            // Load the textures for the piper enemies list
            for (int i = 0; i < piperEnemies.Count; i++)
            {
                Content.Load<Texture2D>("enemy_piper");
                Content.Load<Texture2D>("piper_enemy_map");
            }

            // Load the textures and set position for the walkers array
            for (int i = 0; i < walkers.Length; i++)
                walkers[i] = new Walker(Content.Load<Texture2D>("walker_1"), Content.Load<Texture2D>("walker_map"), RNG, (int) background1_left.m_position.X, (int) background1_right.m_position.X + 2277, -1, 1, 4, 24, 0, 3 );

            // Load all the sound effects
            rocketEngine = Content.Load<SoundEffect>("rocketEngine");
            piperloop = Content.Load<SoundEffect>("piper_loop");
            panic = Content.Load<SoundEffect>("panic");
            shot = Content.Load<SoundEffect>("shot");
            boom = Content.Load<SoundEffect>("boom");
            select = Content.Load<SoundEffect>("select");
            gameOver = Content.Load<SoundEffect>("game_over_sound");
            warning = Content.Load<SoundEffect>("warning_sound");

            // Load the text input box          
            testInput = new InputBox(Content.Load<Texture2D>("TextboxUI"), Content.Load<SpriteFont>("debug"));
        }      
            
        protected override void UnloadContent()
        {
            
        }


        protected override void Update(GameTime gameTime)
        {
            // Now we check our CurrentGameState to see what we should be updating.
            switch (_state)
            {
                case GameState.Title:                    
                    UpdateTitle(gameTime);                    
                    break;

                case GameState.Instructions:
                    UpdateInstructions(gameTime);
                    break;

                case GameState.Gameplay:
                    UpdateGameplay(gameTime);
                    break;

                case GameState.HighScore:                    
                    UpdateHighScore(gameTime);                  
                    break;

                case GameState.Gameover:                                       
                    UpdateGameover(gameTime);
                    resetGame();
                    break;
            }

            base.Update(gameTime);
        }

        // In here is the actual update code used in the Title Screen State
        void UpdateTitle (GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();                     

            testInput.clearText();                              // Clear the text input box from any previous entry
            pad1_old = pad1_curr;
            pad1_curr = GamePad.GetState(PlayerIndex.One);

            keyb1_old = keyb1_curr;
            keyb1_curr = Keyboard.GetState();

            // On pressing the Enter key, play the SELECT sound and change the state to the Instructions state
            if (keyb1_curr.IsKeyDown(Keys.Enter) && keyb1_old.IsKeyUp(Keys.Enter))
            {
                selectInstance = select.CreateInstance();    
                selectInstance.Play();
                _state = GameState.Instructions;
            }

            // On pressing the Start button on the pad, play the SELECT sound and change the state to the Instructions state
            if ((pad1_curr.Buttons.Start == ButtonState.Pressed) && (pad1_old.Buttons.Start == ButtonState.Released))
            {
                selectInstance = select.CreateInstance();    
                selectInstance.Play();
                _state = GameState.Instructions;
            }
        }

        // In here is the actual update code used in the Instructions Screen State
        void UpdateInstructions(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            pad1_old = pad1_curr;
            pad1_curr = GamePad.GetState(PlayerIndex.One);

            keyb1_old = keyb1_curr;
            keyb1_curr = Keyboard.GetState();

            // On pressing the Enter key, play the SELECT sound and change the state to the Gameplay state
            if (keyb1_curr.IsKeyDown(Keys.Enter) && keyb1_old.IsKeyUp(Keys.Enter))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Gameplay;
            }

            // On pressing the Start button on the pad, play the SELECT sound and change the state to the Gameplay state
            if ((pad1_curr.Buttons.Start == ButtonState.Pressed) && (pad1_old.Buttons.Start == ButtonState.Released))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Gameplay;
            }
        }

        // Method for entering the text from the keyboard 
        void UpdateTextInput()
        {
            kb = Keyboard.GetState();
            typedText = testInput.UpdateMe();            
                if (testInput.AmTracking() == true)
                {
                    testInput.TrackString(false);
                    Debug.WriteLine("Your name: " + typedText);
                }
                else
                    testInput.TrackString(true);            
            oldkb = kb;
        }

        // In here is the actual update code used in the Highscore Input State
        void UpdateHighScore(GameTime gameTime)
        {
            // Stop any sounds that are still playing
            if ((panicInstance != null))
                panicInstance.Stop();
            if ((piperloopInstance != null))
                piperloopInstance.Stop();
            if ((rocketEngineInstance != null))
                rocketEngineInstance.Pause();
            if ((shotInstance != null))
                shotInstance.Stop();
            if ((boomInstance != null))
                boomInstance.Stop();
            if ((warningInstance != null))
                warningInstance.Stop();

            // Quit the game if Escape or Back button are pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            pad1_old = pad1_curr;
            pad1_curr = GamePad.GetState(PlayerIndex.One);

            keyb1_old = keyb1_curr;
            keyb1_curr = Keyboard.GetState();

            // Set the invasionmeter to 100%
            invasionmeter = 100;

            // If score is bigger than highscore, they allow for inputing the name from the keyboard
            if (newHSheck(data) == true)
            UpdateTextInput();

            // On pressing the Start button on the pad, play the SELECT sound and change the state to the GameOver state
            if ((pad1_curr.Buttons.Start == ButtonState.Pressed) && (pad1_old.Buttons.Start == ButtonState.Released))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Gameover;
            }

            // On pressing the Enter key, play the SELECT sound and change the state to the GameOver state
            if (keyb1_curr.IsKeyDown(Keys.Enter) && keyb1_old.IsKeyUp(Keys.Enter))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Gameover;
            }
        }

        // In here is the actual update code used in the Game Over and Highscore Table Display State
        void UpdateGameover(GameTime gameTime)
        {
            // Save the new highscore to the Highscore table
            SaveHighScore();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            pad1_old = pad1_curr;
            pad1_curr = GamePad.GetState(PlayerIndex.One);

            keyb1_old = keyb1_curr;
            keyb1_curr = Keyboard.GetState();

            // On pressing the Enter key, play the SELECT sound and change the state to the Title state
            if (keyb1_curr.IsKeyDown(Keys.Enter) && keyb1_old.IsKeyUp(Keys.Enter))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Title;
            }

            // On pressing the Start button on the pad, play the SELECT sound and change the state to the Title state
            if ((pad1_curr.Buttons.Start == ButtonState.Pressed) && (pad1_old.Buttons.Start == ButtonState.Released))
            {
                selectInstance = select.CreateInstance();
                selectInstance.Play();
                _state = GameState.Title;
            }
            
            // Stop any sounds that are still playing
            if ((panicInstance != null))
                panicInstance.Stop();
            if ((piperloopInstance != null))
                piperloopInstance.Stop();
            if ((rocketEngineInstance != null))
                rocketEngineInstance.Pause();
            if ((shotInstance != null))
               shotInstance.Stop();
            if ((boomInstance != null))
                boomInstance.Stop();
            if ((warningInstance != null))
                warningInstance.Stop();
        }

        // In here is the actual update code used in the main Gameplay State
        void UpdateGameplay(GameTime gameTime)
        {
            pad1_old = pad1_curr;
            pad1_curr = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyb1_old = keyb1_curr;
            keyb1_curr = Keyboard.GetState();

            // Update the scrolling background layers, by a horizontal scrolling value which is related to the plane's horizontal speed
            background1_left.updateMe((int)player.horSpeed);
            background1_right.updateMe((int)player.horSpeed);
            background2_left.updateMe(player.horSpeed * 0.9f);
            background2_right.updateMe(player.horSpeed * 0.9f);
            background3_left.updateMe(player.horSpeed * 0.8f);
            background3_right.updateMe(player.horSpeed * 0.8f);
            background4_left.updateMe(player.horSpeed * 0.7f);
            background4_right.updateMe(player.horSpeed * 0.7f);
            background_sky.updateMe(0);

            // update the player (plane) movement
            player.updateMe(keyb1_curr, pad1_curr, pad1_old, (int)background1_left.m_position.X, (int)background1_left.m_position.Y);

            // STOPPING EDGES
            // add opposing speed to the player's horizontal speed once it reaches either end of the map
            if (background1_left.m_position.X > -100)
                player.horSpeed += 12;

            if (background1_right.m_position.X < -2227 + graphics.PreferredBackBufferWidth)
                player.horSpeed -= 12;

            // update the piper
            piper.updateme(-(int)player.horSpeed, (int)background1_left.m_position.X, (int)background1_left.m_position.Y);

            // update the bullets list
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].updateme();
            }

            // remove bullets if outside of screen
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].m_position.X > graphics.PreferredBackBufferWidth || bullets[i].m_position.X < 0)
                    bullets.RemoveAt(i);
            }

            // SHOOTING MECHANICS
            // sets the direction of the moving bullet depending on the orientation of the plane
            // it reads the direction of the plane from the current animation frame of the plane
            if ((keyb1_curr.IsKeyDown(Keys.Z) && keyb1_old.IsKeyUp(Keys.Z)) || ((pad1_curr.Buttons.A == ButtonState.Pressed) && (pad1_old.Buttons.A == ButtonState.Released)))
            {
                if (player.m_animCell.X < 300)
                {
                    bullets.Add(new Bullet(Content.Load<Texture2D>("bullet"), (int)player.m_position.X + 64, (int)player.m_position.Y + 32, 30));
                    shotInstance = shot.CreateInstance();    // create shot sound instance
                    shotInstance.Play();                    // play shot sound instance
                }

                else if (player.m_animCell.X > 300)
                {
                    bullets.Add(new Bullet(Content.Load<Texture2D>("bullet"), (int)player.m_position.X - 40, (int)player.m_position.Y + 32, -30));
                    shotInstance = shot.CreateInstance();     // create shot sound instance
                    shotInstance.Play();                     // play shot sound instance
                }
            }

            // PIPER ENEMY picking up piper                     
            // when the red enemy intersects with the piper, set the piper's position relative to the red enemy, and make red enemy move up
            for( int i = 0; i < piperEnemies.Count; i++)
            {
                if (piperEnemies[i].rect.Intersects(piper.m_rect))
                {
                    piperEnemies[i].m_velocity.Y = -2;
                    piper.m_position.Y = piperEnemies[i].m_position.Y + 55;
                    piper.m_position.X = piperEnemies[i].m_position.X + 76;
                    break;
                }
            }
                        
            // Update the red enemy movement
            for (int i = 0; i < piperEnemies.Count; i++)
                piperEnemies[i].updateme(RNG, graphics.PreferredBackBufferHeight - 100, player.horSpeed * -1, (int)background1_left.m_position.X, (int)background1_left.m_position.Y);

            // Spawn the red enemy above the piper position in random intervals based on the spawn rate
            if (petimeTillSpawn < 0)
            {
                for (int i = 0; i < penemiesPerSpawn; i++)

                {
                    piperEnemies.Add(new EnemyPiper(Content.Load<Texture2D>("enemy_piper"), Content.Load<Texture2D>("piper_enemy_map"), RNG, (int) piper.m_position.X - 70));
                }

                if (pespawnRate < PEBASESPAWNRATE / 2)
                {
                    pespawnRate = PEBASESPAWNRATE;
                    penemiesPerSpawn++;
                }
                else
                {
                    pespawnRate -= 0.1f;
                }
                petimeTillSpawn = pespawnRate;
            }
            else
            {
                petimeTillSpawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // PIPER ENEMY EXPLOSION
            // on bullet colliding with the red enemy destroy both and create an explosion
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < piperEnemies.Count; j++)
                {
                    if (bullets[i].m_rect.Intersects(piperEnemies[j].rect))
                    {
                        if (piperEnemies[j].m_velocity.Y >= 0)
                            score += 500;                           // Add 500 points to score
                        if (piperEnemies[j].m_velocity.Y < 0)
                            score += 500;                           // Add 500 points to score
                        explosions.Add(new Explosion(explosion.m_SpriteSheet, (int)piperEnemies[j].m_position.X - 80, (int)piperEnemies[j].m_position.Y - 125, 7, 24));
                        bullets.RemoveAt(i);
                        piperEnemies.RemoveAt(j);
                        boomInstance = boom.CreateInstance();    // create boom sound instance
                        boomInstance.Play();                    // play boom sound instance

                        break;
                    }
                }
            }

            // update the enemies movement
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].updateme(RNG, graphics.PreferredBackBufferHeight - 100, player.horSpeed * -1, (int)background1_left.m_position.X, (int)background1_left.m_position.Y);

            // update the walkers movement
            for (int i = 0; i < walkers.Length; i++)
                walkers[i].updateme((int)-player.horSpeed, (int)background1_left.m_position.X, (int)background1_left.m_position.Y);

            // SPAWNING ENEMIES
            // Spawn the enemmies in random positions at random intervals based on the spawn rate
            if (timeTillSpawn < 0)
            {
                for (int i = 0; i < enemiesPerSpawn; i++)
                {
                    enemies.Add(new Enemy(Content.Load<Texture2D>("enemy"), Content.Load<Texture2D>("enemy_map"), RNG, (int)background1_left.m_position.X + 200, (int)background1_right.m_position.X + 2000));
                }

                if (spawnRate < BASESPAWNRATE / 2)
                {
                    spawnRate = BASESPAWNRATE;
                    enemiesPerSpawn++;
                }
                else
                {
                    spawnRate -= 0.2f;
                }
                timeTillSpawn = spawnRate;
            }
            else
            {
                timeTillSpawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // EXPLOSION MECHANICS
            // on bullet colliding with the enemy destroy both and create an explosion
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (bullets[i].m_rect.Intersects(enemies[j].rect))
                    {
                        if (enemies[j].m_velocity.Y >= 0)
                            score += 100;                       // add 100 points to score
                        if (enemies[j].m_velocity.Y < 0)
                            score += 100;
                        explosions.Add(new Explosion(explosion.m_SpriteSheet, (int)enemies[j].m_position.X - 80, (int)enemies[j].m_position.Y - 125, 7, 24));
                        bullets.RemoveAt(i);
                        enemies.RemoveAt(j);
                        boomInstance = boom.CreateInstance();    // create boom sound instance
                        boomInstance.Play();                    // play boom sound instance

                        break;
                    }
                }
            }

            // update the scrolling of the expolsions
            for (int i = 0; i < explosions.Count; i++)
                explosions[i].updateMe(player.horSpeed * -1);

            // remove the exposion once it´s animation is finished
            for (int i = 0; i < explosions.Count; i++)
            {
                if (explosions[i].m_animCell.X >= 700)
                {
                    explosions.RemoveAt(i);
                }
            }

            // if the enemy leaves upper edge of the screen, remove the enemy and add 5% to invasionmenter
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].m_position.Y < -128)
                {
                    invasionmeter += 5;
                    enemies.RemoveAt(i);
                    break;
                }
            }

            // if the obducted walker leaves the upper edge of the screen, put him back on ground on the left side of the map and make him walk right
            for (int i = 0; i < walkers.Length; i++)
            {
                if (walkers[i].m_position.Y < -16)
                {
                    walkers[i].m_position.X = background1_left.m_position.X;
                    walkers[i].m_position.Y = 500;
                    walkers[i].walkerSpeed = 1;
                    break;
                }
            }

            // if walker falls below the ground level, put him on the ground level and make him walk right
            for (int i = 0; i < walkers.Length; i++)
            {
                if (walkers[i].m_position.Y > 500)
                {
                    walkers[i].m_position.Y = 500;
                    walkers[i].walkerSpeed = 1;
                    break;
                }
            }

            // if the walker is walking left and reaches the left edge of the map, turn him around
            for (int i = 0; i < walkers.Length; i++)
            {
                if (walkers[i].m_position.X < background1_left.m_position.X)
                {
                    walkers[i].walkerSpeed = 1;
                    break;
                }
            }

            // if the walker is walking right and reaches the right edge of the map, turn him around
            for (int i = 0; i < walkers.Length; i++)
            {
                if (walkers[i].m_position.X > background1_right.m_position.X + 2277)
                {
                    walkers[i].walkerSpeed = -1;
                    break;
                }
            }

            // ENEMY picking up walker             
            // when the enemy intersects with the walker, set the walker's position relative to the enemy, and make enemy move up
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = 0; j < walkers.Length; j++)
                {
                    if (enemies[i].rect.Intersects(walkers[j].m_rect))
                    {
                        if ((enemies[i].m_velocity.Y > 0) && (walkers[j].m_position.Y > 499))
                        {
                            invasionmeter += 1;                     // Add 1% to invasionmenter
                        }
                        enemies[i].m_velocity.Y = -3;
                        walkers[j].m_position.Y = enemies[i].m_position.Y + 55;
                        walkers[j].m_position.X = enemies[i].m_position.X + 76;

                        break;
                    }
                }
            }

            // ENEMY picking up piper             
            // when the enemy intersects with the piper, set the piper's position relative to the enemy, and make enemy move up
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].rect.Intersects(piper.m_rect))
                {
                    enemies[i].m_velocity.Y = -3;
                    piper.m_position.Y = enemies[i].m_position.Y + 55;
                    piper.m_position.X = enemies[i].m_position.X + 76;
                    break;
                }
            }

            // Adding max number to invasionmeter once the piper has left the screen
            if (piper.m_position.Y < 0)
                invasionmeter += 100;

            // Playing the warning sound once the invasionmeter is above 80%
            if (invasionmeter > 80)
            {
                if (warningInstance == null)

                {
                    warningInstance = warning.CreateInstance();
                    warningInstance.IsLooped = true;
                    warningInstance.Play();
                }
            }
                
            // ENGINE SOUND
            // play the engine sound in a loop, adjust the pitch of the sound to match the plane´s current speed
            if (rocketEngineInstance == null)
            {
                rocketEngineInstance = rocketEngine.CreateInstance();
                rocketEngineInstance.IsLooped = true;
                rocketEngineInstance.Play();
            }
            else rocketEngineInstance.Resume();
           
            if (rocketEngineInstance != null)
            {
                if (Math.Abs(player.horSpeed) <= 12)
                {
                    rocketEngineInstance.Pitch = -1f + Math.Abs((player.horSpeed) / 14);
                }
                else rocketEngineInstance.Pitch = -1f + Math.Abs(12) / 14;
            }                               

            // PIPER SOUND
            // play the piper sound loop, when the piper is on ground
            // when the piper is above ground, stop the piper sound and play the crowd panic sound
            if (piperloopInstance == null)
            {
                piperloopInstance = piperloop.CreateInstance();
                piperloopInstance.IsLooped = true;
                piperloopInstance.Play();
            }

            if (panicInstance == null)
            {
                panicInstance = panic.CreateInstance();
                panicInstance.IsLooped = false;
            }

            if (piper.m_position.Y <= 490)
            {
                if (piperloopInstance != null)
                {
                    piperloopInstance.Stop();
                    if (panicInstance != null)
                        panicInstance.Play();
                }
            }

            if (piper.m_position.Y > 490)
            {
                if (piperloopInstance != null)
                {
                    piperloopInstance.Play();
                    panicInstance.Stop();
                }
            }

            // don´t let the invasionmeter go over 100%
            meter.updateme(invasionmeter);

            if (invasionmeter > 100)
                invasionmeter = 100;

            // if invasionmeter gets to 100%, play the Game Over sound and change the game state to Highscore state            
            if (invasionmeter == 100)
            {
                gameOverInstance = gameOver.CreateInstance();
                gameOverInstance.Play();
                _state = GameState.HighScore;
            }

            // FOR TESTING ONLY
            // press the Q key to finish the gameplay state and change to Highscore state
            if (keyb1_curr.IsKeyDown(Keys.Q) && keyb1_old.IsKeyUp(Keys.Q))
            {
                gameOverInstance = gameOver.CreateInstance();
                gameOverInstance.Play();
                _state = GameState.HighScore;
            }            
        }
                
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            // Now we check our CurrentGameState again to see what we should be drawing.
            switch (_state)
            {
                case GameState.Title:
                    DrawTitle(gameTime);
                    break;

                case GameState.Instructions:
                    DrawInstructions(gameTime);
                    break;

                case GameState.Gameplay:
                    DrawGameplay(gameTime);
                    break;

                case GameState.HighScore:
                    DrawGameplay(gameTime);
                    DrawHighScore(gameTime);
                    break;

                case GameState.Gameover:
                    DrawGameover(gameTime);
                    break;
            }
        }
        
        // In here is the actual draw code used in the Title Screen State
        void DrawTitle(GameTime gameTime)
        {
            spriteBatch.Begin();
            title.drawMe(spriteBatch);                      // Draw the Title page background
            pressStart.Drawme(spriteBatch, gameTime);       // Draw the pulsating START button
            spriteBatch.End();
        }

        // In here is the actual draw code used in the Instructions Screen State
        void DrawInstructions(GameTime gameTime)
        {
            spriteBatch.Begin();
            instructions.drawMe(spriteBatch);               // Draw the Instructions page background
            pressStart.Drawme(spriteBatch, gameTime);       // Draw the pulsating START button
            spriteBatch.End();
        }

        // draw the text input 
        void DrawTextInput()
        {                                   
            testInput.DrawMe(spriteBatch, new Vector2(370, 300));                        
        }

        // In here is the actual draw code used in the Highscore Screen State
        void DrawHighScore(GameTime gameTime)
        {
            spriteBatch.Begin();
            gameOverSign.Drawme(spriteBatch);

            // if new highscore is found, draw the text input box
            if (newHSheck(data) == true)
            {
                newScore.Drawme(spriteBatch);
                DrawTextInput();
            }
            else pressStart.Drawme(spriteBatch, gameTime);
            
            spriteBatch.End();
        }

        // In here is the actual draw code used in the Game Over Screen State
        void DrawGameover(GameTime gameTime)
        {
            spriteBatch.Begin();
            gameover.drawMe(spriteBatch);                        

            // draw the updated highscore board
            scoreboard = makeHighScoreString();
            spriteBatch.DrawString(debug, scoreboard, new Vector2(430, 280), Color.White);
            pressStart.Drawme(spriteBatch, gameTime);

            spriteBatch.End();
        }

        // In here is the actual draw code used in the Gameplay State
        void DrawGameplay(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            // draw all the scrolling backgrounds
            background_sky.drawMe(spriteBatch);
            background4_left.drawMe(spriteBatch);
            background4_right.drawMe(spriteBatch);
            background3_left.drawMe(spriteBatch);
            background3_right.drawMe(spriteBatch);
            background2_left.drawMe(spriteBatch);
            background2_right.drawMe(spriteBatch);
            background1_left.drawMe(spriteBatch);
            background1_right.drawMe(spriteBatch);

            // draw the GO BACK sign if the player reaches the left edge of the map
            if (background1_left.m_position.X > -100)
            {
                goback.m_position.X = 20;
                goback.Drawme(spriteBatch, gameTime, 0);
            }

            // draw the GO BACK sign if the player reaches the right edge of the map
            if (background1_right.m_position.X < -2227 + graphics.PreferredBackBufferWidth)
            {
                goback.m_position.X = graphics.PreferredBackBufferWidth - 180;
                goback.Drawme(spriteBatch, gameTime, 129);
            }
            
            // draw enemies
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].drawme(spriteBatch);

            // draw red enemies
            for (int i = 0; i < piperEnemies.Count; i++)
                piperEnemies[i].drawme(spriteBatch);

            // draw the flashing WARNING sign, when the invasionmeter is over 80%
            if (invasionmeter > 80)
            {
                warningSign.Drawme(spriteBatch, gameTime);
            }

            // draw the player
            player.drawMe(spriteBatch);

            // draw bullets
            for (int i = 0; i < bullets.Count; i++)
                bullets[i].drawMe(spriteBatch);

            // draw walkers
            for (int i = 0; i < walkers.Length; i++)
                walkers[i].drawMe(spriteBatch, gameTime);

            // draw explsions
            for (int i = 0; i < explosions.Count; i++)
                explosions[i].Drawme(spriteBatch, gameTime);

            // draw piper
            piper.drawMe(spriteBatch, gameTime);

            // draw map background
            map.drawMe(spriteBatch);

            // draw all the active elements on the mini map
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].drawMeMap(spriteBatch);

            for (int i = 0; i < piperEnemies.Count; i++)
                piperEnemies[i].drawMeMap(spriteBatch);

            for (int i = 0; i < walkers.Length; i++)
                walkers[i].drawMeMap(spriteBatch);                        

            player.drawMeMap (spriteBatch);

            piper.drawMeMap(spriteBatch);

            // draw the invasionmeter bar on a black background
            blackBackground.drawMe(spriteBatch);
            meter.Drawme(spriteBatch);

            // draw the HUD
            HUD.drawMe(spriteBatch);                    

            // draw the score and invasionmeter values
            spriteBatch.DrawString(debug, + invasionmeter + "%", new Vector2(900, 8), Color.White);
            spriteBatch.DrawString(scoretext, "" + score, new Vector2(10, 30), Color.White);

            // DEBUG TEXT - UNCOMMENT ONLY FOR DEBUGGING!!!     
            //spriteBatch.DrawString(debug, "HORIZONTAL SPEED: " + player.horSpeed, Vector2.Zero, Color.White);
            //spriteBatch.DrawString(debug, "NUMBER OF BULLETS: " + bullets.Count, new Vector2(0,16), Color.White);
            //spriteBatch.DrawString(debug, "NUMBER OF WALKERS: " + walkers.Length, new Vector2(0, 32), Color.White);
            //spriteBatch.DrawString(debug, "NUMBER OF ENEMIES: " + enemies.Count, new Vector2(0, 48), Color.White);

            spriteBatch.End();                       
        }
    }
}
