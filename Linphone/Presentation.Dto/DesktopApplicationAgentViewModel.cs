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
using System.Collections.Generic;

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class DesktopApplicationAgentViewModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public AgentRole Role { get; set; }

        public DesktopApplicationAgentStatus Status { get; set; }

        public string UserId { get; set; }

        public double AverageCallDuration { get; set; }

        public double TotalBreakDuration { get; set; }

        public SipProfileViewModel SipProfile { get; set; }

        public WorkgroupShortViewModel Workgroup { get; set; }

        public IEnumerable<DesktopApplicationAgentPermission> Permissions { get; set; }
    }

    public enum DesktopApplicationAgentPermission
    {
        IncomingCall = 1,
        OutgoingCall = 2,
        CallCampaign = 3
    }

    public class WorkgroupShortViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public enum AgentRole
    {
        CallRespondingAgent = 1,
        TicketingExpertAgent = 2,
        SupervisorAgent = 3,
        TicketDispatcherAgent = 4
    }

    public enum DesktopApplicationAgentStatus
    {
        Ready = 1,
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
