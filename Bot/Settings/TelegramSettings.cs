﻿namespace Bot.Settings;

public class TelegramSettings
{
    public const string SectionName = "Telegram";

    public string Token { get; set; }

    public int AdminId { get; set; }

    public string ApiUrl { get; set; }
}