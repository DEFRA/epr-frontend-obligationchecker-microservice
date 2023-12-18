﻿namespace FrontendObligationChecker.ViewModels.GovUk;

public class LegendViewModel
{
    public bool IsPageHeading { get; set; }

    public string Classes { get; set; } = default!;

    public string Text { get; set; } = default!;

    public string Caption { get; set; } = default;

    public bool HasCaption => !string.IsNullOrEmpty(Caption);
}