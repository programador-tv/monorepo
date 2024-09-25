using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Contracts;
using MassTransit;

using Infrastructure.FileHandling;


namespace Background

{
    public class LiveConversionQueue(IVideoHandling handler) : IConsumer<LiveConversionMessage>
    {
        public async Task Consume(ConsumeContext<LiveConversionMessage> context)
        {
            try
            {
        
                await handler.ProcessMP4Async(context.Message.Id, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
    }
}