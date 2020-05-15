using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurtazaAssignement
{
    using System.Collections;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            int[] roomNumbers = new[] {4, 6, 8, 10, 12};
            Hotel hotel = new Hotel(roomNumbers); // hotel with room numbers...


            int[] days1 = new[] {6, 5, 8, 9, 3, 4};
            string ref1 = "REF1";
            int room4 = 4;
            bool result = hotel.RoomBooked(days1, room4);
            if (!result) // false means there was no booked room for given days & room number.
            {
                bool bookingResult = hotel.BookRoom(ref1, days1, room4);
            }
            


            int[] days2 = new[] { 7, 10, 18, 9, 13, 5 };
            bool result2 = hotel.RoomBooked(days2, room4);



            int[] days3 = new[] { 7, 10, 18, 90, 13, 5 };
            

            bool updateResult = hotel.UpdateBooking(ref1, days3, room4);

            hotel.CancelBooking(ref1);


            string ref2 = "REF2";

            bool bookRoomresult = hotel.BookRoom(ref2, days3, room4);




        }
    }
}

