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



        ///receive board, and index of cell return count of horizontal live cells
        public int horCheck(cell[] board, int i)
        {

            int count = 0;
            if (board[i+1].life == 1)
            {
                count++;
            }
            if (board[i-1].life == 1)
            {
                count++;
            }
            return count;
        }

        

        ///return a count of how many vertical cells are live
        public int vertCheck(cell[] board, int i)
        {
            int count = 0;
            if (board[i+18].life == 1)
            {
                count++;
            }
            if (board[i - 18].life == 1)
            {
                count++;
            }
            return count;
        }


        ///check if two cells are diagnoally adjacent

        public int diagCheck(cell[] board, int i)
        {
            int count = 0;
            if (board[i+19].life == 1)
            {
                count++;
            }
            if (board[i + 17].life == 1)
            {
                count++;
            }
            if (board[i - 19].life == 1)
            {
                count++;
            }
            if (board[i - 17].life == 1)
            {
                count++;
            }
            return count;
        }

        ///game logic:
        //loop through all relevent cells:

        public void checkBoard(cell[] board)
        {
            ///example/////////////////
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

                    int liveNeighbors = horCheck(board, x) + vertCheck(board, x) + diagCheck(board, x);
                    if (board[x].life == 1)
                    {
                        //if live cell
                        
                        if (liveNeighbors == 2 || liveNeighbors == 3)
                        {
                            //live cell stays alive
                            newBoard[x].life = 1;
                        } else
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
                        } 
                        else
                        {
                            newBoard[x].life = 0;
                        }
                        
                    }

                  

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