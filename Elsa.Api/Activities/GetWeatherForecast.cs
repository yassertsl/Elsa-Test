﻿using Elsa.Api.Entity;
using Elsa.Extensions;
using Elsa.Workflows;

namespace Elsa.Api.Activities;

public class GetWeatherForecast : CodeActivity<WeatherForecast>
{
    //protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    //{
    //    var apiClient = context.GetRequiredService<IWeatherApi>();
    //    var forecast = await apiClient.GetWeatherAsync();
    //    context.SetResult(forecast);
    //}
}