﻿using tech_project_back_end.Helpter;

namespace tech_project_back_end.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailrequest);
    }
}
