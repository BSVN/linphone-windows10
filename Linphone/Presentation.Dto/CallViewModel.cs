/*
CallViewModel.cs
Copyright (C) 2022 Resa alongside with Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class CallViewModel
    {
        public Guid Id { get; set; }

        public string CallerPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string OperatorFullName { get; set; }

        public Guid OperatorId { get; set; }

        public CallState State { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime FinishedAt { get; set; }

        public TimeSpan Duration { get; set; }

        public Guid? TicketId { get; set; }

        public CallReason? CallReason { get; set; }
    }

    public enum CallState
    {
        Ringing = 1,
        Accepted = 2,
        Missed = 3,
        NormalCleared = 5,
        Terminated = 5
    }

    public enum CallReason
    {
        Callback = 1,
        Question = 2,
        Objection = 3,
        TicketFollowUp = 4,
        Other = 5
    }
}
