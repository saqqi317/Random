namespace MurtazaAssignement
{
    using System.Collections.Generic;

    /// <summary> Represents the hotel object that contains rooms. </summary>
    public class Hotel
    {

        /// <summary> Maps the room number with the room object. </summary>
        private readonly Dictionary<int, Room> roomMap;

        private readonly Dictionary<string, int> bookingRefRoomNumMap;
        public Hotel(int[] roomNums)
        {
            bookingRefRoomNumMap = new Dictionary<string, int>();
            this.roomMap = new Dictionary<int, Room>();
            foreach (int roomNumber in roomNums)
            {
                roomMap.Add(roomNumber, new Room(roomNumber));
            }
        }

        public bool RoomBooked(int[] days, int roomNumber)
        {
            Room room = roomMap[roomNumber];
            int[] bookedDays = room.GetAllBookedDays();
            foreach (int day in days)
            {
                foreach (int bookingDay in bookedDays)
                {
                    if (day == bookingDay)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool BookRoom(string bookingRef, int[] days, int roomNum)
        {
            Room roomToBook = roomMap[roomNum];

            if (!RoomBooked(days, roomNum))
            {
                roomToBook.Book(bookingRef, days);
                bookingRefRoomNumMap.Add(bookingRef, roomNum);
                return true;
            }

            return false;
        }

        public bool UpdateBooking(string bookingRef, int[] days, int roomNum)
        {
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            Room roomToUpdate = roomMap[roomNum];

            int[] bookedDays = roomToUpdate.GetAllBookedDays(bookingRef);

            foreach (int bookedDay in bookedDays)
            {
                foreach (int newDay in days)
                {
                    if (newDay == bookedDay)
                    {
                        return false;
                    }
                }
            }

            roomToUpdate.Book(bookingRef, days);
            return true;
        }

        public void CancelBooking(string bookingRef)
        {
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            int roomNum =  bookingRefRoomNumMap[bookingRef];

            Room bookingToCancel = roomMap[roomNum];
            bookingToCancel.CancelBooking(bookingRef);
            bookingRefRoomNumMap.Remove(bookingRef);
        }
    }
}