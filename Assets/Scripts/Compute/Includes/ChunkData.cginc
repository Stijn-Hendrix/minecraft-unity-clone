#ifndef CHUNK_DATA
#define CHUNK_DATA

int _ChunkSizeHeight;
int _ChunkSizeWidth;

int indexFromCoord(int x, int y, int z)
{
    return x + _ChunkSizeWidth * (y + _ChunkSizeHeight * z);
}

#endif