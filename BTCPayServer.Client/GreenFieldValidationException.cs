﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using BTCPayServer.Client.Models;

namespace BTCPayServer.Client
{
    public class GreenFieldValidationException : Exception
    {
        public GreenFieldValidationException(Models.GreenfieldValidationError[] errors) : base(BuildMessage(errors))
        {
            ValidationErrors = errors;
        }

        private static string BuildMessage(GreenfieldValidationError[] errors)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));
            StringBuilder builder = new StringBuilder();
            foreach (var error in errors)
            {
                builder.AppendLine($"{error.Path}: {error.Message}");
            }
            return builder.ToString();
        }

        public Models.GreenfieldValidationError[] ValidationErrors { get; }
    }
}
