using System;
using Steamworks;

class Program
{
    static void Main(string[] args)
    {
        if (!SteamAPI.Init())
        {
            Console.WriteLine("SteamAPI 初始化失敗");
            return;
        }

        AppId_t appId = new AppId_t(730);

        // 持續刪除直到清單為空
        while (true)
        {
            int favCount = SteamMatchmaking.GetFavoriteGameCount();
            Console.WriteLine($"目前 Favorites/History 總數: {favCount}");

            if (favCount == 0)
                break;

            // 注意：每次都從 index=0 開始刪，避免索引位移漏掉
            for (int i = favCount - 1; i >= 0; i--)
            {
                AppId_t favAppId;
                uint ip;
                ushort connPort, queryPort;
                uint flags, timeLastPlayed;

                if (SteamMatchmaking.GetFavoriteGame(i, out favAppId, out ip, out connPort, out queryPort, out flags, out timeLastPlayed))
                {
                    SteamMatchmaking.RemoveFavoriteGame(favAppId, ip, connPort, queryPort, flags);

                    string type = (flags & 1) != 0 ? "History" : "Favorites";
                    string ipStr = new System.Net.IPAddress(ip).ToString();
                    Console.WriteLine($"刪除 {type}[{i}] - AppID: {favAppId}, IP: {ipStr}, Port: {connPort}, QueryPort: {queryPort}");
                }
            }
        }

        // 最終確認
        int newCount = SteamMatchmaking.GetFavoriteGameCount();
        if (newCount == 0)
            Console.WriteLine("✅ Favorites/History 已完全清空！");
        else
            Console.WriteLine($"⚠️ 清理後仍剩餘數量: {newCount}");

        SteamAPI.Shutdown();
        Console.WriteLine("程式結束，請按任意鍵退出...");
        Console.ReadKey(); // 等待使用者按鍵
    }
}