namespace MurtazaAssignement
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary> Represents the hotel object that contains rooms. </summary>
    public class Hotel
    {

        /// <summary> Maps the room number with the room object. </summary>
        private readonly Dictionary<int, Room> roomMap;
        private readonly Dictionary<string, List<int>> bookingRefRoomNumMap;

        public Hotel(int[] roomNums)
        {
            bookingRefRoomNumMap = new Dictionary<string, List<int>>();
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

                List<int> roomNumbers = new List<int>();
                roomNumbers.Add(roomNum);
                bookingRefRoomNumMap.Add(bookingRef, roomNumbers);
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

            if (CanUpdate(bookingRef, days, roomNum))
            {
                roomToUpdate.Book(bookingRef, days);
                return true;
            }

            return false;
        }


        private bool CanUpdate(string bookingRef, int[] days, int roomNum)
        {
            Room roomToUpdate = roomMap[roomNum];

            // Get all the booked days for roomNum room skipping this bookingRef 
            // because we do not want to check the booking reference that we want to update.
            int[] bookedWithOtherBookingRef = roomToUpdate.GetAllBookedDays(bookingRef);

            // bookedWithOtherBookingRef holds all the booked days for roomNum room number except bookingRef booking reference.

            // loop through each booked day to check if there is any match found with input days array...
            foreach (int bookedDay in bookedWithOtherBookingRef)
            {
                foreach (int nextDay in days)
                {
                    if (nextDay == bookedDay)
                    {
                        // we have found the match with other booking ref than the one we want to update
                        // therefore, we cant update this booking ref.
                        return false;
                    }
                }
            }

            return true;
        }

        public void CancelBooking(string bookingRef)
        {
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            List<int> roomNums =  bookingRefRoomNumMap[bookingRef];

            foreach (int nextRoomNum in roomNums)
            {
                Room bookingToCancel = roomMap[nextRoomNum];
                bookingToCancel.CancelBooking(bookingRef);
            }

            // once all the rooms booking is cancelled, then remove the reference from bookingRefRoomNumMap hashtable.
            bookingRefRoomNumMap.Remove(bookingRef);

        }

        public bool RoomsBooked(int[] days, int[] roomNums)
        {
            foreach (int nextRoomNum in roomNums)
            {
                if (this.RoomBooked(days, nextRoomNum))
                {
                    return true;
                }
            }

            return false;
        }

        public bool BookRooms(string bookingRef, int[] days, int[] roomNums)
        {
            if (!RoomsBooked(days, roomNums))
            {
                // this means we can book given days in given room numbers....
                foreach (int nextRoomNum in roomNums)
                {
                    Room roomToBook = roomMap[nextRoomNum];
                    roomToBook.Book(bookingRef, days);
                }

                bookingRefRoomNumMap.Add(bookingRef, roomNums.ToList());

                return true;
            }

            return false;
        }

        public bool UpdateBooking(string bookingRef, int[] days, int[] roomNums)
        {
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            // loop through the list or rooms and check if update is possible..
            foreach (int nextRoomNumber in roomNums)
            {
                if(!CanUpdate(bookingRef, days, nextRoomNumber))
                {
                    return false;
                }
            }

            // since we can update all bookings now, lets do it.
            foreach (int nextRoomNumber in roomNums)
            {
                Room roomToUpdate = roomMap[nextRoomNumber];
                roomToUpdate.Book(bookingRef, days);
            }

            return true;
        }
    }
}