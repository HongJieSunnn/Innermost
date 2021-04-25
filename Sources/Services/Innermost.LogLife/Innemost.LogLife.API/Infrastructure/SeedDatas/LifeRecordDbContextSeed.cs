using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Infrastructure.SeedDatas
{
    public class LifeRecordDbContextSeed
    {
        public async Task SeedAsync(LifeRecordDbContext context,IConfiguration configuration)
        {
            var seeders = GetDefaultLifeRecords();
            var locations = GetDefaultLocations();
            var musicRecords = GetDefaultMusicRecord();

            await context.Locations.AddRangeAsync(locations);
            await context.LifeRecords.AddRangeAsync(seeders);
            await context.MusicRecords.AddRangeAsync(musicRecords);

            await context.SaveChangesAsync();
        }

        List<LifeRecord> GetDefaultLifeRecords()
        {
            return new List<LifeRecord>
            {
                new LifeRecord(
                    "HongJieSun","Test for article","I am HongJieSun.This is my default test text of an article.😋",
                    TextType.Article,1,13,false,"/TestArticle",DateTime.Now),
                new LifeRecord(
                    "HongJieSun","Test for essay","I am HongJieSun.This is my default test text of an essay.🤯",
                    TextType.Essay,2,12,false,"/TestEssay",DateTime.Now),
                new LifeRecord(
                    "HongJieSun","HongJieSun's Mine","等好多年后你问我为什么会跟你在一起 我希望我的回答一定会是很简单的:" +
                    "我遇到了你 然后喜欢上了你 没有什么其他奇怪的要求 爱情不就该这么简单吗 我喜欢你你也喜欢我就够了 我真的很渴望这样的爱情 " +
                    "它太简单了 简单到我们甚至不愿意承认这样的爱情真的存在 也许只能在动漫里 让我们向往一下而已 可我真的 太羡慕动漫里的情节了 " +
                    "他们就是 我遇到了你 我喜欢你 仅此而已 我不知道青春是不是已经从高中毕业了的我身边溜走了 如果不是我真的希望我可以在青春还没溜走的现在 " +
                    "遇到你 喜欢上你 我知道心动是什么样的感觉 虽然好久没心动过了 但是我相信你总会到来 然后让我快发臭的心动一动 如果过去了 那我也无话可说了 " +
                    "我太胆小了 太自卑了 我以为恋爱就像动漫里一样 我喜欢你 你就会也喜欢我 事实证明并不是这样 可惜我又爱自作多情 就这样我的青春不知不觉溜走了 但是没事 " +
                    "一切都取决于自己的看法 不是吗 有什么规定说青春什么时候会结束啊 哈哈 那既然这样我真的希望你快点出现 也许是再从我身边经过 然后我对你说我喜欢你 我喜欢你 你也喜欢我 " +
                    "就够了 还需要什么吗 如果还需要 那一定是我带着你 一起去海边 看着蓝天白云 大海 你说 :哇 好漂亮 我说 是啊 白天天和海都是蓝色的 很漂亮 都是一望无垠 阴天时海会失去生机 " +
                    "因为灰蒙蒙的天空也看起来很压抑 它们依赖依靠着对方 晚上的时候天很黑 我们看不清楚海 我们却听得到海浪声 海浪声是那样的动人 总给困扰的人带来安抚 给急躁的人带来灵感 给疲倦的人带来睡意 " +
                    "而我想爱情很多时候也是这样 我喜欢你 你也喜欢我 因为缘分 因为很多相似之处 我们在一起 但人没办法永远保持一样的心情 我喜欢你 但我有时候其他的心情盖过了我喜欢你的心情" +
                    " 相当于天黑了 但我喜欢你 不管怎样我都是喜欢你的 天再黑海浪也不会停的 一样的 我对你的喜欢也不会停 我依然会像海浪一样 因为我喜欢你 不管是\"白天\"还是\"黑夜\" 我都是喜欢你的 " +
                    "也许这是该多考虑的 人的心情不可能时时相同 但某个信念 或者说是某些烙印在心里的东西是不会变的 唉 我总是这样 太爱写一些 想一些太美好的事 但是人都是向往美好的 不是吗 喜欢你也是件美好的事 " +
                    "只是人不可能每时每刻都处在美好的环境下 我必须得等 而我现在只能羡慕 也许也在告诉自己 不要让你让青春溜走 保持自己的观点:两个人 我喜欢你 你也喜欢我 就够了 就是美好的事 就是让我向往的事",
                    TextType.Article,1,12,false,"/TestArticle",DateTime.Now),
            };
        }

        List<Location> GetDefaultLocations()
        {
            return new List<Location>()
            {
                new Location(1,"福建省", "福州市", "连江县", "东湖镇", "我家"),
                new Location(2, "江苏省", "南京市", "浦口区", "", "南京工业大学"),
            };
        }

        List<MusicRecord> GetDefaultMusicRecord()
        {
            return new List<MusicRecord>()
            {
                new MusicRecord(12,"借口", "周杰伦", "七里香"),
                new MusicRecord(13, "你听得到", "周杰伦", "叶惠美"),
            };
        }
    }
}
