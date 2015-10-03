using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace First_Game
{
    public class Level
    {
        public char[,] LevelArray = new char[Game.RoomHeight/Game.TileSize, Game.RoomWidth/Game.TileSize];
        public Level(char[] PassedLevelArray)
        {
            int RoomWidth = Game.RoomWidth;
            int RoomHeight = Game.RoomHeight;
            int TileSize = Game.TileSize;
            for (int height = 0; height < (RoomHeight / TileSize); height++)
                for (int width = 0; width < (RoomWidth / TileSize); width++)
                        LevelArray[height, width] = PassedLevelArray[height * (RoomWidth / TileSize) + width];
        }
    }
}
