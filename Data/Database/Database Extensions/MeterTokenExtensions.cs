using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    public static class MeterTokenExtensions
    {
        public static async Task UsedMeterToken(this DatabaseContext context, MeterToken token)
        {
            var usedToken = await context.MeterTokens.FirstOrDefaultAsync(mt => mt.TokenID == token.TokenID);
            usedToken.Used = true;
            context.MeterTokens.Update(usedToken);
            await context.SaveChangesAsync();
        }

        public static async Task<MeterToken> FindMeterTokenByID(this DatabaseContext context,string tokenID)
        {
            return await context.MeterTokens.FirstOrDefaultAsync(mt => mt.TokenID == tokenID);
        }

        public static async Task<MeterToken> FindUnusedMeterToken(this DatabaseContext context,string tokenID)
        {
            var token = await context.MeterTokens.FirstOrDefaultAsync(mt => mt.TokenID == tokenID);
            if(token == null)
                return null;
            if(token.Used)
                return null;
            return token;
        }
    }
}