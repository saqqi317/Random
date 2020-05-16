namespace MurtazaAssignement
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary> Represents the hotel object that contains rooms. </summary>
    public class Hotel
    {
        /// <summary> Maps the room number with the room object. </summary>
        private readonly Dictionary<int, Room> roomMap;

        /// <summary> Maps the reference with list of the room numbers that booking reference represents. </summary>
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

        /// <summary> Checks if the room is already booked for given days array. </summary>
        /// <param name="days">Array of days to check.</param>
        /// <param name="roomNumber">Room number.</param>
        /// <returns>A value indicating if (even for a single day in given array) room is already booked. True means its booked, false otherwise.</returns>
        public bool RoomBooked(int[] days, int roomNumber)
        {
            if (days == null || days.Length == 0)
            {
                // if no days are given then there is nothing to check.
                // return early.
                return false;
            }

            // get the room object using given room number...
            // we can throw an exception if the roomNumber is invalid, but its not in the spec document.
            // Therefore, it is assumed that the roomNumber is valid.
            Room room = roomMap[roomNumber]; // constant lookup..

            // Get all the days that are booked for given room.
            HashSet<int> bookedDays = room.GetAllBookedDays();

            if (bookedDays.Count == 0)
            {
                // if no days are booked then there is not need to check for given days.
                // we can return early from this method.
                return false;
            }

            // loop through each day and check if we have a match. 
            // This is O(n) - linear because we are checking elements of days array in a hashset.
            // This should be improved but due to time limitation, im leaving it as is for now.
            foreach (int day in days)
            {
                // bookedDays hashset gives us constant time lookup.
                if (bookedDays.Contains(day))
                {
                    return true;
                }
            }

            // returning false means we do not find any booked room for given days.
            return false;
        }

        /// <summary> Books the room with given booking reference for the days for the room number. </summary>
        /// <param name="bookingRef">The booking reference to keep track of days and room.</param>
        /// <param name="days">Array containing the days to book.</param>
        /// <param name="roomNum">The room number to book.</param>
        /// <returns>A value indicating if the booking was successful. True means successful, false otherwise.</returns>
        public bool BookRoom(string bookingRef, int[] days, int roomNum)
        {
            // using RoomBooked method to check if this room is available for given days.
            if (!RoomBooked(days, roomNum))
            {
                // once we are here it means we have unbooked days for this room number..
                // so lets book this room.

                // get the room object using given room number...
                // we can throw an exception if the roomNumber is invalid, but its not in the spec document.
                // Therefore, it is assumed that the roomNumber is valid.
                Room roomToBook = roomMap[roomNum];

                // since we have already checked if this room has given days available 
                // we can just call it to book.
                roomToBook.Book(bookingRef, days);

                // update bookingRefRoomNumMap mapper to keep track of booking reference with the rooms.
                // we need list of rooms because in Cancel booking method, we only have the booking reference 

                List<int> roomNumbers = new List<int>();
                roomNumbers.Add(roomNum);
                bookingRefRoomNumMap.Add(bookingRef, roomNumbers);
                return true;
            }

            // returning false means we could not find the vacant room for given array of days.
            return false;
        }

        /// <summary> Updates the existing booking.  </summary>
        /// <param name="bookingRef">The booking reference to find existing booking that needs updating.</param>
        /// <param name="days">The new array of days to change to.</param>
        /// <param name="roomNum">The room number to update the booking.</param>
        /// <returns>A value indicating if the update booking was successful. True means successful, false otherwise.</returns>
        public bool UpdateBooking(string bookingRef, int[] days, int roomNum)
        {
            // first we check if the booking reference is valid.
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            // Get the room object that needs updating.
            Room roomToUpdate = roomMap[roomNum];

            // check if we can update the rooms for given reference and room number.
            if (CanUpdate(bookingRef, days, roomNum))
            {
                // go ahead and update the existing booking.
                roomToUpdate.Book(bookingRef, days);
                return true;
            }

            return false;
        }

        /// <summary> Determines if existing booking can be update with given days. </summary>
        /// <param name="bookingRef">Existing booking reference.</param>
        /// <param name="days">New days to update.</param>
        /// <param name="roomNum">The room number.</param>
        /// <returns>A value indicating if its okay to update. True means it can be update, false otherwise.</returns>
        private bool CanUpdate(string bookingRef, int[] days, int roomNum)
        {
            Room roomToUpdate = roomMap[roomNum];

            // Get all the booked days for roomNum room skipping this bookingRef 
            // because we do not want to check the booking reference that we want to update.
            HashSet<int> bookedWithOtherBookingRef = roomToUpdate.GetAllBookedDays(bookingRef);

            if (bookedWithOtherBookingRef.Count == 0)
            {
                return true;
            }

            // bookedWithOtherBookingRef holds all the booked days for roomNum room number except bookingRef booking reference.
            // loop through each day from the input and check if there is any match found in the already booked days...
            foreach (int nextDay in days)
            {
                if (bookedWithOtherBookingRef.Contains(nextDay))
                {
                    // we have found the match with other booking ref than the one we want to update
                    // therefore, we cant update this booking ref.
                    return false;
                }
            }

            // return true when there is not conflict between all the days that are booked for the room (roomNum)
            // and given array of days.
            return true;
        }

        /// <summary> Cancels the existing booking, removes it from the system. </summary>
        /// <param name="bookingRef">The booking reference to remove.</param>
        public void CancelBooking(string bookingRef)
        {
            // check if given booking reference is a valid one.
            if (!bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                throw new NoSuchBookingException();
            }

            // since we are not given the room number, we have to get all those room numbers for which this booking reference was made.
            List<int> roomNums = bookingRefRoomNumMap[bookingRef];

            // loop through each room and cancel the booking.
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
            if (bookingRefRoomNumMap.ContainsKey(bookingRef))
            {
                // throw exception because bookingRef must be a new booking reference.
                // the client must update if existing booking reference is used.
            }
            
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
                if (!CanUpdate(bookingRef, days, nextRoomNumber))
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