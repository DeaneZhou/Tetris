using System;
using Tetris.Models;

namespace Tetris.Helper
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };
        private readonly Random random = new Random();

        public Block NextBlock { get; private set; }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            } while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
