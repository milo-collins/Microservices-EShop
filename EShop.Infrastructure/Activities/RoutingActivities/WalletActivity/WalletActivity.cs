﻿using EShop.Infrastructure.Command.Wallet;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Activities.RoutingActivities.WalletActivity
{
    public class WalletActivity : IActivity<TransactMoney, TransactMoneyLog>
    {
        // Rollback steps
        async Task<CompensationResult> ICompensateActivity<TransactMoneyLog>.Compensate(CompensateContext<TransactMoneyLog> context)
        {
            try
            {
                // Add funds back to wallet
                var addFunds = new AddFunds
                { UserId = context.Log.UserId, CreditAmount = context.Log.Amount };
                var endpoint = await context.GetSendEndpoint(new Uri("rabbitmq://localhost/add_funds"));
                await endpoint.Send(addFunds);

                return context.Compensated();
            }
            catch (Exception)
            {
                // Failed compensation
                return context.Failed();
            }
        }

        // Completes transaction
        async Task<ExecutionResult> IExecuteActivity<TransactMoney>.Execute(ExecuteContext<TransactMoney> context)
        {
            try
            {
                var deductFunds = new DeductFunds() { UserId = context.Arguments.UserId, DebitAmount = context.Arguments.Amount };
                var endpoint = await context.GetSendEndpoint(new Uri("rabbitmq://localhost/deduct_funds"));
                await endpoint.Send(deductFunds);
                var transactMoneyLog = new TransactMoney()
                {
                    UserId = context.Arguments.UserId,
                    Amount = context.Arguments.Amount
                };
                return context.CompletedWithVariables<TransactMoneyLog>(transactMoneyLog, new { });
            }
            catch(Exception ex)
            {
                // Activity faulted
                return context.Faulted();
            }
        }
    }

    public class TransactMoney
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransactMoneyLog
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
    }
}
