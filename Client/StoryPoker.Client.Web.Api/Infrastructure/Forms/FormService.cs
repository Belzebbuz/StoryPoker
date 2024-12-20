﻿using ErrorOr;
using StoryPoker.Client.Web.Api.Abstractions.Forms;

namespace StoryPoker.Client.Web.Api.Infrastructure.Forms;

public class FormService(IServiceProvider serviceProvider) : IFormService
{
    public async Task<ErrorOr<IEnumerable<InputBase>>> GetFormAsync(string key, IDictionary<string, string> parameters)
    {
        if (InMemoryFormCollection.FormGroups.TryGetValue(key, out var formGroup))
        {
            var form = formGroup.ToList();
            foreach (var input in form)
            {
                await input.AttachData(serviceProvider, parameters);
            }
            return form;
        }
        return Error.NotFound();
    }
}
