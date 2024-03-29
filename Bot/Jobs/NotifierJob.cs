﻿using Bot.Extensions;
using Bot.Resources;
using Microsoft.Extensions.Localization;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Bot.Jobs;

[DisallowConcurrentExecution]
public class NotifierJob : IJob
{
    private readonly ITelegramBotClient _bot;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly TakeoutService _takeoutService;
    private readonly UserService _userService;

    public NotifierJob(ITelegramBotClient bot, IStringLocalizer<Messages> localizer, TakeoutService takeoutService,
        UserService userService)
    {
        _bot = bot;
        _localizer = localizer;
        _takeoutService = takeoutService;
        _userService = userService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var sundayDate = DateTime.Now.GetLastSunday();

        var users = await _userService.ListAsync();

        foreach (var user in users)
        {
            var takeoutsSinceSunday = await _takeoutService.ListSinceAsync(user.Id, sundayDate);

            if (!takeoutsSinceSunday.Any())
            {
                continue;
            }

            await _bot.SendTextMessageAsync(new(user.Id),
                _localizer.GetTakeoutsMessage(takeoutsSinceSunday),
                ParseMode.Markdown);
        }
    }
}