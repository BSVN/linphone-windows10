/*
CallViewModel.cs
Copyright (C) 2022 Resaa Corporation.
Copyright (C) 2016 Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
*/

using System;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class OperatorViewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public UserRole Role { get; set; }

        public AgentStatus? Status { get; set; }

        public string UserId { get; set; }

        public SipProfileViewModel SipProfile { get; set; }

        //public WorkgroupViewModel Workgroup { get; set; }
    }

    public enum UserRole
    {
        CallRespondingAgent = 1,
        TicketingExpertAgent = 2,
        Supervisor = 3,
        TicketDispatcher = 4
    }

    public enum AgentStatus
    {
        Ready  = 1,
        Break = 2,
        Offline = 3,
    }

    public class SipProfileViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public SipProtocol Protocol { get; set; }
    }

    public enum SipProtocol
    {
        TCP = 1,
        UDP = 2
    }
}
