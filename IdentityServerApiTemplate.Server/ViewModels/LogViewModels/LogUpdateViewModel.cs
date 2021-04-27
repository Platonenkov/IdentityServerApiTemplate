﻿using Calabonga.EntityFrameworkCore.Entities.Base;

namespace IdentityServerApiTemplate.Server.ViewModels.LogViewModels
{
    /// <summary>
    /// Log View model for Update operations
    /// </summary>
    public class LogUpdateViewModel : ViewModelBase
    {
        public string Logger { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
    }
}