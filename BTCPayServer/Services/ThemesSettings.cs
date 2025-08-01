using System;
using System.ComponentModel.DataAnnotations;
using BTCPayServer.JsonConverters;
using Newtonsoft.Json;

namespace BTCPayServer.Services;

public enum ThemeExtension
{
    [Display(Name = "Does not extend a GRSPay Server theme, fully custom")]
    Custom,
    [Display(Name = "Extends the GRSPay Server Light theme")]
    Light,
    [Display(Name = "Extends the GRSPay Server Dark theme")]
    Dark
}

public class ThemeSettings
{
    [Display(Name = "Use custom theme")]
    public bool CustomTheme { get; set; }

    [Display(Name = "Custom Theme Extension Type")]
    public ThemeExtension CustomThemeExtension { get; set; }

    [JsonConverter(typeof(UnresolvedUriJsonConverter))]
    public UnresolvedUri CustomThemeCssUrl { get; set; }

    [JsonConverter(typeof(UnresolvedUriJsonConverter))]
    public UnresolvedUri LogoUrl { get; set; }

    public bool FirstRun { get; set; } = true;
}
