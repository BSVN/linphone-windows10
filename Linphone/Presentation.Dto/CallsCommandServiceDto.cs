/*
CallsCommandServiceTerminateIncomingResponse.cs
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

namespace BelledonneCommunications.Linphone.Presentation.Dto
{
    public class CallsCommandServiceInitiateIncomingRequest
    {
        public string CallerPhoneNumber { get; set; }

        public string AgentPhoneNumber { get; set; }

        public string InboundService { get; set; }
    }


    public class CallsCommandServiceInitiateIncomingResponse : GenericResponseBase<CallViewModel> { }


    public class CallsCommandServiceInitiateOutgoingRequest
    {
        public string AgentPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }
        
        public string InboundService { get; set; }
    }

    public class CallsCommandServiceInitiateOutgoingResponse : GenericResponseBase<CallViewModel> { }

    public class CallsCommandServiceSubmitMissedIncomingRequest
    {
        public string CallerPhoneNumber { get; set; }

        public string AgentPhoneNumber { get; set; }
        
        public string InboundService { get; set; }
    }

    public class CallsCommandServiceSubmitMissedIncomingResponse : GenericResponseBase<CallViewModel> { }

    public class CallsCommandServiceSubmitMissedOutgoingRequest
    {
        public string AgentSoftPhoneNumber { get; set; }

        public string CalleePhoneNumber { get; set; }

        public string InboundService { get; set; }
    }

    public class CallsCommandServiceSubmitMissedOutgoingResponse : GenericResponseBase<CallViewModel> { }

    public class CallsCommandServiceEstablishResponse : GenericResponseBase<CallViewModel> { }

    public class CallsCommandServiceTerminateResponse : GenericResponseBase<CallViewModel> { }

    public class CallsQueryServiceGetByIdResponse : GenericResponseBase<CallViewModel> { }
}
