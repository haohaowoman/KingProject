// RS485Modbus.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "RS485Modbus.h"
#include <windows.h>

#include "targetver.h"

// RS485Modbus.cpp : 定义 DLL 应用程序的导出函数。
//

#include <Strsafe.h>


#define WIN32_LEAN_AND_MEAN             //  从 Windows 头文件中排除极少使用的信息
// Windows 头文件
/* CRC 高位字节值表 */
static unsigned char auchCRCHi[] = {
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
} ;
/* CRC低位字节值表*/
static unsigned char auchCRCLo[] = {
	0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
	0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
	0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
	0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
	0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
	0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
	0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
	0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
	0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
	0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
	0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
	0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
	0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
	0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
	0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
	0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
	0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
	0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
	0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
	0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
	0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
	0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
	0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
	0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
	0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
	0x43, 0x83, 0x41, 0x81, 0x80, 0x40
} ;
/*
struct SendSeal16
{
	//unsigned char add1;			//从机地址
	unsigned char functionCode;	//功能码
	unsigned short jcqAddr;		//寄存器起始地址
	unsigned short counts;		//寄存器个数/数据个数
	unsigned char bytes;		//字节数
	unsigned char* outData;		//输出值
	unsigned short* jcqData;	//寄存器值
	unsigned int dataLen;		//outData或者jcqData的数据个数
	unsigned char CRCCode[2];	//CRC校验码

	bool bHighBefore;			//CRC码高位在前
	bool bPolynomail;			//采用多项式计算
	unsigned short polynomail;  //多项式校验码，默认0xa001

	unsigned char* p_seal;		//协议的数组
	unsigned int len;			//协议的长度
/ *
	unsigned char* recv;		//接收返回的结果
	unsigned int recvLen;		//返回结果的长度
	
	bool InitSeal(int m_len,unsigned int counts = 1,unsigned int* dataArr = NULL,bool bHighBefore = true,bool bPolynomail = false,unsigned short polynomail = 0xa001)
	{
		p_seal = new unsigned char[m_len];
		this->len = m_len;
		this->counts = counts;
		this->dataArr = dataArr;
		this->bHighBefore = bHighBefore;
		this->bPolynomail = bPolynomail;
		this->polynomail = polynomail;
		if(NULL != p_seal)
			return TRUE;
		else
			return FALSE;
	}
	unsigned int GetSealLen(){return this->len;}
	void SetHighBefore(bool bBefore){this->bHighBefore = bBefore;}
	bool GetHighBefore(){return this->bHighBefore;}
	void SetBPolyNomail(bool bPoly){this->bPolynomail = bPoly;};
	bool GetBPolyNomail(){return this->bPolynomail;}
	void SetNomail(bool bNomail){this->polynomail = bNomail;}
	bool GetNomail(){return this->polynomail;}
	void FreeSeal()
	{
		if (NULL != p_seal)
		{
			delete[] p_seal;
			p_seal = NULL;
		}
	}* /

};*/
struct ReadInfo
{
	unsigned char funcCode;
	unsigned short startAddr;
	unsigned short num;
};
union byteToInt
{
	int i;
	unsigned char c[4];
};
union byteToShort
{
	unsigned short s;
	unsigned char c[2];
};


static char m_fjAddr = 0x01;

static unsigned char* GetCRC16(unsigned char* Cmd,int len,bool IsHighBefore)
{
	int index;
	int crc_Low = 0xFF;
	int crc_High = 0xFF;
	for (int i = 0; i < len; i++)
	{
		index = crc_High ^ (char)Cmd[i];
		crc_High = crc_Low ^ auchCRCHi[index];
		crc_Low = (unsigned char)auchCRCLo[index];
	}
	unsigned char* p = new unsigned char[2];
	if (IsHighBefore == true)
	{
		p[0] = (unsigned char)crc_High;
		p[1] = (unsigned char)crc_Low;
	}
	else
	{
		p[0] = (unsigned char)crc_Low;
		p[1] = (unsigned char)crc_High;
	}
	return p;
}

//查表计算CRC
static unsigned char* GetCRC16Full(unsigned char* Cmd, int len,bool IsHighBefore)
{
	unsigned char* check = GetCRC16(Cmd,len, IsHighBefore);
	return check;
}

static short CRC(unsigned char* iBuffer,unsigned short polynomail = 0xa001)
{
	short crcValue = 0xff;
	unsigned char v;
	int i = 0;
	for(;iBuffer[i] != NULL;i++)
	{
		v = iBuffer[i];
		crcValue ^= v;

		for (int i = 0; i < 8; i++ )
		{
			int temp = crcValue & 1;
			crcValue = (short)(crcValue >> 1);

			if (1 == temp)
			{
				crcValue ^= polynomail;
			}
		}
	}

	return crcValue;
}
static unsigned char* GetCRC16ByPoly(unsigned char* Cmd,int len,unsigned short Poly = 0xa001, bool IsHighBefore = true)
{
	unsigned char* CRC = new unsigned char[2];
	unsigned short CRCValue = 0xFFFF;
	for (int i = 0; i < len; i++)
	{
		CRCValue = (unsigned short)(CRCValue ^ Cmd[i]);
		for (int j = 0; j < 8; j++)
		{
			if ((CRCValue & 0x0001) != 0)
			{
				CRCValue = (unsigned short)((CRCValue >> 1) ^ Poly);
			}
			else
			{
				CRCValue = (unsigned short)(CRCValue >> 1);
			}
		}
	}
	byteToShort m_si;
	m_si.s = CRCValue;
	unsigned char* p = new unsigned char[2];
	if (IsHighBefore == true)
	{
		p[0] = (CRCValue >> 8) & 0xff;
		p[1] = CRCValue & 0xff;

	}
	else
	{
		p[0] = CRCValue & 0xff;
		p[1] = (CRCValue >> 8) & 0xff;

	}
	return p;
}

//多项式计算CRC校验
static unsigned char* GetCRC16ByPolyFull(unsigned char* Cmd, int len,unsigned short Poly,bool IsHighBefore = true)
{
	unsigned char* check = GetCRC16ByPoly(Cmd,len, Poly, IsHighBefore);

	return check;
}

static unsigned char* SealWriteCharCommand(unsigned char funcCode,unsigned short jcqAddr,unsigned short counts,unsigned char bytes,unsigned char* outData,unsigned int len,bool bHighBefore,bool bPolynomail,unsigned short polynomail)
{
	unsigned char* crcBuffer = NULL;
	int n = 0;
	crcBuffer = new unsigned char[len + 9];
	crcBuffer [0] = m_fjAddr;
	crcBuffer [1] = funcCode;
	crcBuffer [2] =	(unsigned char)((jcqAddr >> 8) & 0xff);
	crcBuffer [3] =	(unsigned char)(jcqAddr & 0xff);
	crcBuffer [4] =	(unsigned char)((counts >> 8) & 0xff);
	crcBuffer [5] =	(unsigned char)(counts & 0xff);
	crcBuffer [6] = bytes;

	for (unsigned i = 0;i < len;i++)
	{
		crcBuffer [7 + i] =	outData[i];
	}

	n = len + 7;

	unsigned char* tempBuffer;

	if (bPolynomail)
	{
		tempBuffer = GetCRC16ByPolyFull(crcBuffer, n,polynomail,bHighBefore);
	}
	else
	{
		tempBuffer = GetCRC16Full(crcBuffer,n,bHighBefore);
	}
	crcBuffer[n] = tempBuffer[0];
	crcBuffer[n+1] = tempBuffer[1];

	return crcBuffer;
}

static unsigned char* SealWriteShortCommand(unsigned char funcCode,unsigned short jcqAddr,unsigned short counts,unsigned char bytes,unsigned short* outData,unsigned int len,bool bHighBefore,bool bPolynomail,unsigned short polynomail)
{
	unsigned char* crcBuffer = NULL;
	int n = 0;
	crcBuffer = new unsigned char[len + 9];
	crcBuffer [0] = m_fjAddr;
	crcBuffer [1] = funcCode;
	crcBuffer [2] =	(unsigned char)((jcqAddr >> 8) & 0xff);
	crcBuffer [3] =	(unsigned char)(jcqAddr & 0xff);
	crcBuffer [4] =	(unsigned char)((counts >> 8) & 0xff);
	crcBuffer [5] =	(unsigned char)(counts & 0xff);
	crcBuffer [6] = bytes;

	for (unsigned int i = 0;i < len;i++)
	{
		crcBuffer [7 + i*2] = (unsigned char)((outData[i] >> 8) & 0xff);
		crcBuffer [8 + i*2] = (unsigned char)(outData[i] & 0xff);

	}

	n = len*2 + 8;	

	unsigned char* tempBuffer;

	if (bPolynomail)
	{
		tempBuffer = GetCRC16ByPolyFull(crcBuffer, n,polynomail,bHighBefore);
	}
	else
	{
		tempBuffer = GetCRC16Full(crcBuffer,n,bHighBefore);
	}
	crcBuffer[n] = tempBuffer[0];
	crcBuffer[n+1] = tempBuffer[1];

	return crcBuffer;
}

static unsigned char* SealReadRigisterCommand(unsigned char funcCode,unsigned short startRegisterAddr,unsigned short counts,bool bHighBefore, bool bPolynomail ,unsigned short polynomail)
{
	unsigned char* crcBuffer = new unsigned char[8];
	crcBuffer [0] = m_fjAddr;
	crcBuffer [1] = funcCode;
	crcBuffer [2] =	(unsigned char)((startRegisterAddr >> 8) & 0xff);
	crcBuffer [3] =	(unsigned char)(startRegisterAddr & 0xff);
	crcBuffer [4] =	(unsigned char)((counts >> 8) & 0xff);
	crcBuffer [5] =	(unsigned char)(counts & 0xff);

	unsigned char* tempBuffer;
	int len = 6;
	if (bPolynomail)
	{
		tempBuffer = GetCRC16ByPolyFull(crcBuffer, len,polynomail,bHighBefore);
	}
	else
	{
		tempBuffer = GetCRC16Full(crcBuffer,len,bHighBefore);
	}
	crcBuffer[len] = tempBuffer[0];
	crcBuffer[len+1] = tempBuffer[1];
	return crcBuffer;
}

RS485MODBUS_API bool GetSealCommand( SendSeal16* m_seal )
{
	if (m_seal->functionCode == 0x01 || m_seal->functionCode == 0x02 || m_seal->functionCode == 0x03 || m_seal->functionCode == 0x04 || m_seal->functionCode == 0x05 || m_seal->functionCode == 0x06)
	{
		m_seal->p_seal = SealReadRigisterCommand(m_seal->functionCode,m_seal->jcqAddr,m_seal->counts,m_seal->bHighBefore,m_seal->bPolynomail,m_seal->polynomail);
		m_seal->len = 8;
	}
	else if (m_seal->functionCode == 0x0F )  //写多个线圈
	{
		m_seal->p_seal = SealWriteCharCommand(m_seal->functionCode,m_seal->jcqAddr,m_seal->counts,m_seal->bytes,m_seal->outData,m_seal->dataLen,m_seal->bHighBefore,m_seal->bPolynomail,m_seal->polynomail);
		m_seal->len = m_seal->dataLen + 9;
	}
	else if(m_seal->functionCode == 0x10)  //写多个寄存器
	{
		m_seal->p_seal = SealWriteShortCommand(m_seal->functionCode,m_seal->jcqAddr,m_seal->counts,m_seal->bytes,m_seal->jcqData,m_seal->dataLen,m_seal->bHighBefore,m_seal->bPolynomail,m_seal->polynomail);
		m_seal->len = (m_seal->dataLen)*2 + 9;
	}
	return TRUE;
}

/*分析读状态的值*/

RS485MODBUS_API void AnalysisRecvSeal( unsigned char* recv,unsigned int recvLen,SendSeal16* m_seal )
{
	if (recv == NULL || recvLen <= 0)
	{
		return;
	}
	byteToShort m_bs;
	m_seal->bError = false;
	m_seal->recvDataLen = 0;
	if (recv[1] == 0x01 || recv[1] == 0x02)
	{
		m_seal->recvFuncCode = recv[1];
		m_seal->recvBytes = recv[2];
		m_seal->state = new unsigned char[recvLen-2];
		for (unsigned int i = 0;i < recvLen - 2;i++)
		{
			m_seal->state[i] = recv[i+2];
			m_seal->recvDataLen++;
		}
	}
	else if (recv[1] == 0x03 || recv[1] == 0x04)
	{
		m_seal->recvFuncCode = recv[1];
		m_seal->recvBytes = recv[2];

		m_seal->data = new unsigned short[m_seal->recvBytes];

		m_seal->state = new unsigned char[recvLen-2];


		for(int i= 0; i<(m_seal->recvBytes)-1;i++)
		{
			m_bs.c[1] = recv[i*2+3];
			m_bs.c[0] = recv[i*2+4];
			m_seal->state[i] = (unsigned char)m_bs.s;
			m_seal->recvDataLen++;
		}

		m_seal->recvData=m_bs.s;

	}
	else if (recv[1] == 0x05 || recv[1] == 0x06 || recv[1] == 0x0f || recv[1] == 0x10)
	{
		m_seal->recvFuncCode = recv[1];
		m_bs.c[0] = recv[2];
		m_bs.c[1] = recv[3];
		m_seal->recvOutAddr = m_bs.s;

		m_bs.c[0] = recv[4];
		m_bs.c[1] = recv[5];
		m_seal->recvData = m_bs.s;
	}
	else if (recv[0] > 128)
	{
		m_seal->bError = true;
		m_seal->errorFuncCode = recv[0];
		m_seal->errorCode = recv[1];
	}
}


RS485MODBUS_API bool InitSeal( SendSeal16* m_seal,unsigned char functionCode,unsigned short jcqAddr,unsigned short counts,unsigned char bytes,
							  unsigned char* outData,unsigned short* jcqData,unsigned int dataLen,bool bHighBefore,
							  bool bPolynomail ,unsigned short polynomail )
{
	if (functionCode == 0x01 || functionCode == 0x02 || functionCode == 0x03 || functionCode == 0x04 || functionCode == 0x05 || functionCode == 0x06)
	{
		m_seal->functionCode = functionCode;
		m_seal->jcqAddr = jcqAddr;
		m_seal->counts = counts;
		m_seal->p_seal = new unsigned char[8];
		m_seal->len = 8;
	}
	else if (functionCode == 0x0f || functionCode == 0x10)
	{
		m_seal->functionCode = functionCode;
		m_seal->jcqAddr = jcqAddr;
		m_seal->counts = counts;
		m_seal->bytes = bytes;
		m_seal->outData = outData;
		m_seal->dataLen = dataLen;
		m_seal->p_seal = new unsigned char[dataLen + 9];
		m_seal->len = dataLen + 9;
	}
	else if (functionCode == 0x0f || functionCode == 0x10)
	{
		m_seal->functionCode = functionCode;
		m_seal->jcqAddr = jcqAddr;
		m_seal->counts = counts;
		m_seal->bytes = bytes;
		m_seal->outData = outData;
		m_seal->jcqData = jcqData;
		m_seal->dataLen = dataLen;
		m_seal->p_seal = new unsigned char[dataLen*2 + 9];
		m_seal->len = dataLen*2 + 9;
	}
	m_seal->bHighBefore = bHighBefore;
	m_seal->bPolynomail = bPolynomail;
	m_seal->polynomail = polynomail;

	return TRUE;
}

RS485MODBUS_API unsigned int GetSealLen(SendSeal16* m_seal)
{
	return m_seal->len;
}
RS485MODBUS_API void SetHighBefore(SendSeal16* m_seal,bool bBefore)
{
	m_seal->bHighBefore = bBefore;
}
RS485MODBUS_API bool GetHighBefore(SendSeal16* m_seal)
{
	return m_seal->bHighBefore;
}
RS485MODBUS_API void SetBPolyNomail(SendSeal16* m_seal,bool bPoly)
{
	m_seal->bPolynomail = bPoly;
}
RS485MODBUS_API bool GetBPolyNomail(SendSeal16* m_seal)
{
	return m_seal->bPolynomail;
}
RS485MODBUS_API void SetFjAddr(char addr)
{
	m_fjAddr = addr;
}
RS485MODBUS_API void SetNomail(SendSeal16* m_seal,unsigned short bNomail)
{
	m_seal->polynomail = bNomail;
}
RS485MODBUS_API unsigned short GetNomail(SendSeal16* m_seal)
{
	return m_seal->polynomail;
}
RS485MODBUS_API void FreeSeal(SendSeal16* m_seal)
{
	if (NULL != m_seal)
	{
		if (NULL != m_seal->p_seal)
		{
			delete[] m_seal->p_seal;
			m_seal->p_seal = NULL;
		}
		delete m_seal;
		m_seal = NULL;
	}
}