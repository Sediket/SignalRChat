using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {

        public cell[] oldBoard = new cell[324];
        public cell[] newBoard = new cell[324];

        /// <summary>
        /// receives message from clients
        /// </summary>
        /// <param name="user"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string jsonString)
        {

            //populate oldBoard      

            for (int i = 0; i < oldBoard.Length; ++i)
            {
                oldBoard[i] = new cell();
            }

            //populate newBoard blank

            for (int i = 0; i < newBoard.Length; ++i)
            {
                newBoard[i] = new cell();
                newBoard[i].life = 0;
                newBoard[i].color = "#000000";
            }


            //add in cell data for the gameboard from the jsonString to oldBoard

            int count = 0;

            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                //JsonElement studentsElement = root.GetProperty("Students");
                foreach (JsonElement cell in root.EnumerateArray())
                {
                    cell.TryGetProperty("life", out JsonElement lifeElement);
                    oldBoard[count].life = lifeElement.GetInt32();
                    //Console.WriteLine(lifeElement.ToString());
                    cell.TryGetProperty("color", out JsonElement colorElement);
                    oldBoard[count].color = colorElement.ToString();
                    //Console.WriteLine(colorElement.ToString());
                    count++;
                }
            }

            checkBoard(oldBoard);

            ///receive board in json
            ///deserialize the board from json
            ///do board calculations on a new board
            ///serialize the new board back into json
            ///send to all clients

            await Clients.All.SendAsync("ReceiveMessage", user, newBoard);
        }



        public static string avgHexColor(string v1, string v2)
        {
            int red = int.Parse(v1.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(v1.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(v1.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            string avgColor = "#" + (red / 2).ToString("X2") + (green / 2).ToString("X2") + (blue / 2).ToString("X2");
            return avgColor;
        }


        ///receive board, and index of cell return count of horizontal live cells
        public static string[] horCheck(cell[] board, int i)
        {
            int count = 0;
            string hexColor = board[i].color;
            if (board[i + 1].life == 1)
            {
                count++;
                hexColor = avgHexColor(board[i + 1].color, hexColor);
            }
            if (board[i - 1].life == 1)
            {
                hexColor = avgHexColor(board[i + 1].color, hexColor);
                count++;
            }
            string[] retArr = { count.ToString(), hexColor };
            return retArr;
        }





        ///return a count of how many vertical cells are live
        public static string[] vertCheck(cell[] board, int i)
        {
            int count = 0;
            string hexColor = board[i].color;
            if (board[i + 18].life == 1)
            {
                hexColor = avgHexColor(board[i + 18].color, hexColor);
                count++;
            }
            if (board[i - 18].life == 1)
            {
                hexColor = avgHexColor(board[i - 18].color, hexColor);
                count++;
            }
            string[] retArr = { count.ToString(), hexColor };
            return retArr;
        }

        ///check if two cells are diagnoally adjacent

        public static string[] diagCheck(cell[] board, int i)
        {
            int count = 0;
            string hexColor = board[i].color;
            if (board[i + 19].life == 1)
            {
                hexColor = avgHexColor(board[i + 19].color, hexColor);
                count++;
            }
            if (board[i + 17].life == 1)
            {
                hexColor = avgHexColor(board[i + 17].color, hexColor);
                count++;
            }
            if (board[i - 19].life == 1)
            {
                hexColor = avgHexColor(board[i - 19].color, hexColor);
                count++;
            }
            if (board[i - 17].life == 1)
            {
                hexColor = avgHexColor(board[i - 17].color, hexColor);
                count++;
            }
            string[] retArr = { count.ToString(), hexColor };
            return retArr;
        }

        ///game logic:
        //loop through all relevent cells:

        public static void checkBoard(cell[] board)
        {

            ///board is 16 by 16, with one extra row and column around the board
            ///makeing the board 18 by 18

            //start at this cell, the first relevent cell
            int x = 19;

            //go until the second to last row
            while (x < 18 * 16)
            {
                if (x % 18 == 0 || x % 18 == 17)
                {
                    x++;
                }
                else
                {
                    //valid cells, check:
                    //1 Any live cell with two or three neighbors survives.
                    //2 Any dead cell with three live neighbors becomes a live cell.
                    //3 All other live cells die in the next generation.Similarly, all other dead cells stay dead

                    string[] currArr = horCheck(board, x);
                    int liveNeighbors = Int32.Parse(currArr[0]);
                    string hexColor = currArr[1];

                    currArr = vertCheck(board, x);
                    liveNeighbors += Int32.Parse(currArr[0]);
                    hexColor = avgHexColor(currArr[1], hexColor);

                    currArr = diagCheck(board, x);
                    liveNeighbors += Int32.Parse(currArr[0]);
                    hexColor = avgHexColor(currArr[1], hexColor);


                    //int liveNeighbors = horCheck(board, x) + vertCheck(board, x) + diagCheck(board, x);



                    if (board[x].life == 1)
                    {
                        //if live cell

                        if (liveNeighbors == 2 || liveNeighbors == 3)
                        {
                            //live cell stays alive
                            newBoard[x].life = 1;
                            newBoard[x].color = hexColor;
                        }
                        else
                        {
                            //live cell dies
                            newBoard[x].life = 0;
                        }
                    }
                    else
                    {
                        //currently dead cell
                        if (liveNeighbors == 3)
                        {
                            //becomes alive
                            newBoard[x].life = 1;
                            newBoard[x].color = hexColor;
                        }
                        else
                        {
                            newBoard[x].life = 0;
                        }
                    }
                    x++;
                }
            }
            ///example/////////////////
            //int x = 19;
            //while (x < 18 * 17)
            //{
            //    if (x % 18 == 0 || x % 18 == 17)
            //    {
            //        x++;
            //    }
            //    else
            //    {
            //        ///check cell

            //    }
            //}
            ///example/////////////////
        }







    }
}