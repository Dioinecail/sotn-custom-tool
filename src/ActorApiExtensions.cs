using BizHawk.Client.Common;
using SotnApi;
using SotnApi.Constants.Addresses;
using SotnApi.Constants.Values.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ActorApiExtensions
{
    public static List<long> GetAllActors(IMemoryApi memAPI)
    {
        List<long> ActorAddresses = new();

        try
        {
            long start = Game.ActorsStart;
            for (int i = 0; i < Actors.Count; i++)
            {
                long hitboxWidth = memAPI.ReadByte(start + Actors.HitboxWidthOffset);
                long hitboxHeight = memAPI.ReadByte(start + Actors.HitboxHeightOffset);
                long hp = memAPI.ReadU16(start + Actors.HpOffset);
                long damage = memAPI.ReadU16(start + Actors.DamageOffset);
                long sprite = memAPI.ReadU16(start + 22);

                if (hitboxWidth > 0 || hitboxHeight > 0 || hp > 0 || damage > 0 || sprite > 0)
                {
                    ActorAddresses.Add(start);
                }
                start += Actors.Offset;
            }
        }
        catch (NotImplementedException ex)
        {

        }

        return ActorAddresses;
    }
}