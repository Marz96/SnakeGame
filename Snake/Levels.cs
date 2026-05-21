using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SnakeGame
{
    public static class Levels
    {
        private static readonly string[][] RawMaps = new string[][]
        {
            // Уровень 1: Чистое поле (Классика)
            new string[] {
                "                                        ",
                "                                        "
            },

            // Уровень 2: Четыре угла
            new string[] {
                " #####                            ##### ",
                " #                                    # ",
                " #                                    # ",
                "                                        ",
                "                                        ",
                " #                                    # ",
                " #                                    # ",
                " #####                            ##### "
            },

            // Уровень 3: Центральный крест
            new string[] {
                "                    #                   ",
                "                    #                   ",
                "                    #                   ",
                "               ###########              ",
                "                    #                   ",
                "                    #                   ",
                "                    #                   "
            },

            // Уровень 4: Вертикальные колонны
            new string[] {
                "          #              #              ",
                "          #              #              ",
                "          #              #              ",
                "          #              #              ",
                "          #              #              ",
                "          #              #              ",
                "          #              #              ",
                "          #              #              "
            },

            // Уровень 5: Коробка с проходами
            new string[] {
                "  ####################################  ",
                "  #                                  #  ",
                "  #                                  #  ",
                "                                        ",
                "                                        ",
                "  #                                  #  ",
                "  #                                  #  ",
                "  ####################################  "
            },

            // Уровень 6: Зигзаг (Горизонтальные барьеры)
            new string[] {
                " ##################################     ",
                "                                        ",
                "     ################################## ",
                "                                        ",
                " ##################################     "
            },

            // Уровень 7: Лабиринт "Комнаты"
            new string[] {
                "              #          #              ",
                "              #          #              ",
                " ##############          ############## ",
                "                                        ",
                " ##############          ############## ",
                "              #          #              ",
                "              #          #              "
            },

            // Уровень 8: Двойной крест
            new string[] {
                "      #                        #        ",
                "    #####                    #####      ",
                "      #                        #        ",
                "                                        ",
                "      #                        #        ",
                "    #####                    #####      ",
                "      #                        #        "
            },

            // Уровень 9: Спираль
            new string[] {
                " ####################################   ",
                " #                                  #   ",
                " #  ##############################  #   ",
                " #  #                            #  #   ",
                " #  #                            #  #   ",
                " #  ##############################  #   ",
                " #                                  #   ",
                " ####################################   "
            },

            // Уровень 10: Арена смерти (Много мелких блоков)
            new string[] {
                "    ###    ###    ###    ###    ###     ",
                "                                        ",
                "    ###    ###    ###    ###    ###     ",
                "                                        ",
                "    ###    ###    ###    ###    ###     ",
                "                                        ",
                "    ###    ###    ###    ###    ###     "
            }
        };

        public static List<Point> GetObstacles(int levelIndex)
        {
            var obstacles = new List<Point>();
            if (levelIndex < 0 || levelIndex >= RawMaps.Length) return obstacles;

            string[] map = RawMaps[levelIndex];
            int startRow = (GlobalSettings.GridHeight - map.Length) / 2;

            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[y].Length; x++)
                {
                    if (x < GlobalSettings.GridWidth && map[y][x] == '#')
                    {
                        obstacles.Add(new Point(x, startRow + y));
                    }
                }
            }
            return obstacles;
        }
    }
}
