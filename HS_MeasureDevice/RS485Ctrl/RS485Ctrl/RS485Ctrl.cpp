// RS485Ctrl.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "RS485Ctrl.h"
#include "Serial.h"

#include "../RS485Modbus/RS485Modbus.h"
#pragma  comment(lib, "../Debug/RS485Modbus.lib")
#include <setupapi.h>
#pragma comment(lib, "setupapi.lib")

#include <process.h>
#include <map>
#include <iostream>
#include <string>

using namespace std;
static CSerial			m_serial;//串口对象

static BOOL				bOpen = FALSE;
static HANDLE			pThread = NULL;
//static char*			m_com = "COM5";
static LPTSTR           m_com = L"COM5";
static char*			m_comData = NULL; 
static SendSeal16*		m_seal = NULL;
static unsigned char	errorCode = 0xff;
static RunState			runState;
static unsigned char*   recvDate = NULL;	//接收的状态
static unsigned short*  recvData = NULL;	//接收的数据

map<short,string>valueMap;


static float Voltage,Speed,Current,Hz,errMsg; //全局变量 用于不断更新对应的状态信息
static float Fbuf[5];
int timerId = 1;
static unsigned short gnm=0000;
static unsigned short dizhi=0001;
HANDLE m_thread;

bool threadFlag = false;      
static bool ComFlag = false;

static bool erroStaus = false;




static BOOL GetCom( char* strCom )
{
	try{
		char* strInfoClass;
		char* strInfoSz;
		char* strInfoDev;
		BOOL isVerifyTrue = FALSE;

		HDEVINFO hDevInfo;     
		SP_DEVINFO_DATA DeviceInfoData;     
		DWORD i;

		// 得到所有设备 HDEVINFO      
		hDevInfo = SetupDiGetClassDevs(NULL, 0, 0, DIGCF_PRESENT | DIGCF_ALLCLASSES );     

		if (hDevInfo == INVALID_HANDLE_VALUE)     
			return FALSE;     

		// 循环列举     
		DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);  


		for (i = 0; SetupDiEnumDeviceInfo(hDevInfo, i, &DeviceInfoData); i++)     
		{     
			char szClassBuf[MAX_PATH] = { 0 };
			char szDescSzBuf[MAX_PATH] = { 0 };
			char szDescBuf[MAX_PATH] = { 0 };

			// 获取类名  
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &DeviceInfoData, SPDRP_CLASS, NULL, (PBYTE)szClassBuf, MAX_PATH - 1, NULL))         
				continue;

			strInfoClass = szClassBuf;
			//获取设备描述信息
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &DeviceInfoData, SPDRP_DEVICEDESC, NULL, (PBYTE)szDescSzBuf, MAX_PATH - 1, NULL))         
				continue;

			strInfoSz = szDescSzBuf;
			//获取设备描述信息
			if (!SetupDiGetDeviceRegistryProperty(hDevInfo, &DeviceInfoData, SPDRP_FRIENDLYNAME, NULL, (PBYTE)szDescBuf, MAX_PATH - 1, NULL))         
				continue;

			strInfoDev = szDescBuf;


			char* p = NULL;
			if ( "Ports" == strInfoClass &&  strstr(strInfoDev,"COM") )
			{
				p = strstr(strInfoDev,"COM");
				//strncpy(strCom,p,4);
				strncpy_s(p, strlen(strCom),strCom,5);
				break;
			}
		}   
		//  释放     
		SetupDiDestroyDeviceInfoList(hDevInfo); 
		return TRUE;
	}
	catch(...)
	{
		return FALSE;
	}
}

static BOOL CloseSerial()
{
	if (m_serial.Close())
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

static void AnalysisRunState(unsigned short s_state)
{
	runState.RDY_ON = runState.ABOVE_LIMIT = runState.ALARM = runState.AT_SETPOINT = runState.OFF_2_STATUS = runState.OFF_3_STATUS = runState.RDY_REF = false;
	runState.RDY_RUN = runState.REMOTE = runState.SWC_ON_INHIB = runState.TRIPPED = false;
	unsigned short temp = s_state;
	runState.RDY_ON = (bool)(temp & 0x01);
	runState.RDY_RUN = (bool)(temp>>1 & 0x01);
	runState.RDY_REF = (bool)(temp>>2 & 0x01);
	runState.TRIPPED = (bool)(temp>>3 & 0x01);
	runState.OFF_2_STATUS = (bool)(temp>>4 & 0x01);
	runState.OFF_3_STATUS = (bool)(temp>>5 & 0x01);
	runState.SWC_ON_INHIB = (bool)(temp>>6 & 0x01);
	runState.ALARM = (bool)(temp>>7 & 0x01);
	runState.AT_SETPOINT = (bool)(temp>>8 & 0x01);
	runState.REMOTE = (bool)(temp>>9 & 0x01);
	runState.ABOVE_LIMIT = (bool)(temp>>10 & 0x01);

}
static void GetSeal(SendSeal16* m_seal,unsigned char code,unsigned short addr,unsigned short counts)
{
	InitSeal(m_seal,code,addr,counts);
	GetSealCommand(m_seal);
}

static BOOL ReadSingleCoil(char* com, char* comData,unsigned short addr,unsigned short counts ,unsigned char* p)
{
	if(NULL == m_seal)
		m_seal = new SendSeal16;
	GetSeal(m_seal,0x01,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(200);
		memset(&buf, 0, sizeof(buf));
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(200);
		}
	}

	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
}

static BOOL ReadStragglingInput( unsigned short addr,unsigned short counts)
{
	if(NULL == m_seal)
		m_seal = new SendSeal16; 
	GetSeal(m_seal,0x02,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(100);	
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(100);
		}
	}

	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
}

static BOOL ReadHoldRegister( unsigned short addr,unsigned short counts)
{
	if(NULL == m_seal)
		m_seal = new SendSeal16;  
	GetSeal(m_seal,0x03,addr,counts);

	/*注意还是存在数组越界的问题*/
	unsigned char buf[512];

	int n = 0;

	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(50);
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,512);

			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(50);
		}
	}

	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		recvDate = NULL;
		recvData = NULL;
		//FreeSeal(m_seal);
		return FALSE;

	}
	if (m_seal->functionCode == m_seal->recvFuncCode || m_seal->jcqAddr == m_seal->recvOutAddr)
	{

		recvData =&(m_seal->recvData);
		recvDate = m_seal->state;
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
	return FALSE;
}

static BOOL ReadImputRegister( unsigned short addr,unsigned short counts )
{
	if(NULL == m_seal)
		m_seal = new SendSeal16;  
	GetSeal(m_seal,0x04,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(100);
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(100);
		}
	}

	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
}

static BOOL WriteSingleCoil( unsigned short addr,unsigned short counts )
{
	if(NULL == m_seal)
		m_seal = new SendSeal16; 
	GetSeal(m_seal,0x05,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(100);	
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(100);
		}
	}
	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
}

static BOOL WriteSingleRegister( unsigned short addr,unsigned short counts )
{
	if(NULL == m_seal)
		m_seal = new SendSeal16;  
	GetSeal(m_seal,0x06,addr,counts);

	///注意 数据越界
	unsigned char buf[512];
	memset(&buf,0,sizeof(buf));
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(50);

		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);

			if (n > 0)
			{

				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				//CommunicationReady();
				ComFlag= true;
				break;
			}
			else
			{
				//CommunicationReady();
				ComFlag = false;
			}
			Sleep(50);
		}
	}
	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}

	return FALSE;
}

static BOOL WriteCoils( unsigned short addr,unsigned short counts,unsigned char bytes,unsigned char* output)
{

	if(NULL == m_seal)
		m_seal = new SendSeal16; 
	GetSeal(m_seal,0x0f,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(100);
		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(100);
		}
	}
	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;

		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;

		return TRUE;
	}
}

static BOOL WriteRegisters( unsigned short addr,unsigned short counts,unsigned char bytes,unsigned short* output)
{
	if(NULL == m_seal)
		m_seal = new SendSeal16;
	GetSeal(m_seal,0x10,addr,counts);

	char buf[1024];
	int n = 0;
	if (!bOpen)
		InitSerial(m_com);
	if (bOpen)
	{
		m_serial.SendData((char*)m_seal->p_seal,m_seal->len);
		Sleep(100);

		for (int j = 0;j < 2;j++)
		{
			n = m_serial.ReadData(buf,1024);
			if (n > 0)
			{
				AnalysisRecvSeal((unsigned char*)buf,n,m_seal);
				break;
			}
			Sleep(100);
		}
	}

	if (m_seal->bError)
	{
		errorCode = m_seal->errorCode;
		//FreeSeal(m_seal);
		return FALSE;
	}
	if (m_seal->functionCode == m_seal->recvFuncCode && m_seal->jcqAddr == m_seal->recvOutAddr)
	{
		errorCode = 0xff;
		//FreeSeal(m_seal);
		return TRUE;
	}
}


RS485CTRL_API BOOL SetRevolveRate(unsigned int rate)
{

	if(!WriteSingleRegister(0001,rate))//给定值，对应寄存器地址40002
		return FALSE;
	return TRUE;
}

//3.9 23：41
RS485CTRL_API short GetNowSpeed()
{

	if(!ReadHoldRegister(0145,1))//实际值， 采集转速的
		return -1;
	if(NULL == recvData)
		return -1;
	return *recvData;
}
RS485CTRL_API short GetNowVoltage()  //电压
{
	if(!ReadHoldRegister(0156,1))//实际值， 采集电压的
		return -1;
	if(NULL == recvData)
		return -1;
	return *recvData;

}
RS485CTRL_API short GetNowCurrent()  //电流
{

	if(!ReadHoldRegister(0152,1))//实际值， 采集电流的
		return -1;
	if(NULL == recvData)
		return -1;
	return *recvData;

}
RS485CTRL_API short GetNowTemperRate()
{

	if(!ReadHoldRegister(0202,1))//实际值， 采集温度的

		return -1;
	if(NULL == recvData)
		return -1;
	return *recvData;
}
RS485CTRL_API bool SetTorque( unsigned int rate )
{

	if(!WriteSingleRegister(0001,rate))//给定值，对应寄存器地址40002
		return FALSE;
	return TRUE;
}
RS485CTRL_API short GetNowTorque()
{

	if(!ReadHoldRegister(0004,1))//实际值，对应寄存器地址40005
		return -1;
	if (NULL == recvData)
		return -1;
	return *recvData;
}
RS485CTRL_API BOOL SetFJInit()
{
	//if(!paramTranslate(0000,1142))//控制字，1142对应16进制0x0476，初始化变频器
	//return false;
	return paramTranslate(0000,1142);
}
RS485CTRL_API BOOL SetRest()
{
	//if(!paramTranslate(0000,1270))//控制字，对应寄存器地址40001,1143对应16进制的0x047F，启动风机
	//	return FALSE;
	return paramTranslate(0000,1270);

}


RS485CTRL_API BOOL SetStart()
{

	if(!paramTranslate(0000,1151))//控制字，对应寄存器地址40001,1151对应16进制的0x047F，启动风机
		return FALSE;
	return TRUE;
}
RS485CTRL_API BOOL SetStop()
{
	return paramTranslate(0000,1143);//控制字，对应寄存器地址40001,1143对应16进制的0x0477，停止风机
}
RS485CTRL_API unsigned char GetLastErrorCode()
{
	return errorCode;
}
RS485CTRL_API BOOL GetConnectState()
{	
	for (int i = 0;i < 1;i++)
	{
		if (ReadHoldRegister(0003,1) && (((*recvData) >> 8)&0x01) == 0x01)//状态字，对应寄存器地址40004，读取连接状态，暂定
		{
			return TRUE;
		}
		Sleep(200);

	}
	return FALSE;
}
RS485CTRL_API int GetRunState()
{

	return ReadHoldRegister(0156,1);

}
RS485CTRL_API bool SetHZ(  float rate )
{
	//paramTranslate(0001,(7*(atoi(str))))
	//if(!paramTranslate(0001,((rate*20000)/2950.0)))//给定值，对应寄存器地址40002，rate为频率如10HZ，因为0-50HZ对应数据为0-20000，故ratex400
	//	return FALSE;
	//return TRUE;
	return paramTranslate(0001,(unsigned short)((rate*20000)/2950.0));

}
RS485CTRL_API int GetHZ()
{

	if(!ReadHoldRegister(0151,1))//实际值，对应寄存器地址40005
		return -1;
	if (NULL == recvData)
		return -1;
	return (unsigned int)(*recvData/400.0);//读取到的数据为0-20000，对应0-50HZ
}


RS485CTRL_API bool SetCRC( bool bHighBefore,bool bPoly,unsigned short poly )
{
	if (NULL == m_seal)
		m_seal = new SendSeal16;
	SetHighBefore(m_seal,bHighBefore);
	SetBPolyNomail(m_seal,bPoly);
	SetNomail(m_seal,poly);
	return TRUE;
}
RS485CTRL_API BOOL InitSerial(LPTSTR com)
{
	try
	{

		m_com = com;
		if (m_serial.Open(com))
		{

			AllReady();
			bOpen = TRUE;
			return TRUE;
		}
		else
		{

			AllRelease();

			return FALSE;
		}
	}
	catch (...)
	{
		return FALSE;
	}
}
RS485CTRL_API void ReleaseData()
{
	if (NULL != m_seal)
	{

		delete m_seal;
		m_seal = NULL;
	}
	if (bOpen)
	{
		m_serial.Close();
		bOpen = FALSE;
	}
}
RS485CTRL_API void SetAddr( char addr )
{
	SetFjAddr(addr);
}


RS485CTRL_API const float*  GetVlue()
{

	Fbuf[0]= Voltage;
	Fbuf[1]= Speed;
	Fbuf[2]= Current;
	Fbuf[3]= Hz;
	Fbuf[4]=errMsg;
	return Fbuf;
}

RS485CTRL_API bool OnWrite(unsigned short addr,unsigned short funCode)
{

	/*if (!WriteSingleRegister(addr,funCode))
	{
	return FALSE;
	}*/
	return WriteSingleRegister(addr,funCode) == TRUE ? true:false;
}

void CALLBACK TimerProc(HWND hwnd,UINT uMsg,UINT idEvent,DWORD dwTime)
{
	int a=0;
	a= GetNowVoltage();
	Voltage=a/10.f;
	Sleep(100);

	int b=0;
	b=GetNowSpeed();
	Speed=(b*((1500.f*2)/20000.f));
	Sleep(100);

	int c=0;
	c=GetNowCurrent();
	Current=c/1.f;
	Sleep(100);

	int d=0;
	d=GetHZ();
	Hz=d/1.f;
	Sleep(100);

	bool e;
	e=OnWrite(dizhi,gnm);
	Sleep(100);
}
bool paramTranslate(unsigned short addr,unsigned short funCode)
{
	dizhi=addr;
	gnm=funCode;
	return true;
}



DWORD WINAPI ThreadProc(LPVOID lpParameter)
{
	threadFlag = true;
	while (threadFlag)
	{
		int a=0;
		a= GetNowVoltage();

		Voltage=a/10.f;
		Sleep(100);

		int b=0;
		b=GetNowSpeed();
		//b*((1500.0*2)/20000))
		Speed=(b*(1500.f*2/20000.f));
		Sleep(100);

		int c=0;
		c=GetNowCurrent();
		Current=c/1.f;
		Sleep(100);

		int d=0;
		d=GetHZ();
		Hz=d/1.f;
		Sleep(100);

		bool e;
		e=OnWrite(dizhi,gnm);
		Sleep(100);

		int f;
		f=ErroMessage();

		erroAutoKill(f);

		errMsg=(float)f;
		Sleep(100);

		ProtectPrgam(Speed,gnm);
	}
	return 1;
}
RS485CTRL_API void ProtectPrgam(float b,unsigned short gnm)
{
	if (b < 5.0 && gnm == 0)
	{
		SetStop();
	}

}
RS485CTRL_API void erroAutoKill(int f)
{
	int erroCode=66;
	erroCode=f;
	if (erroCode  == 26241)
	{
		//WriteSingleRegister(0000,1142);   //发送备妥或者 复位的信息
		SetRest();
	} 
	else if(erroCode == 0 && erroStaus == false)
	{
		//WriteSingleRegister(0000,1143);   //发送停止的信息 让备妥灯亮起
		SetStop();
		erroStaus =true;
	}
	else
	{

	}
}
void InitMyFirstThread()
{
	int a=0;
	m_thread=CreateThread(NULL,0,ThreadProc,&a,0,NULL);

}
void ExitThread()
{
	threadFlag=false;
	CloseHandle(m_thread);
}
bool   AllRelease()
{
	ExitThread();
	return TRUE;

}
bool   AllReady()
{
	InitMyFirstThread();
	return TRUE;
}

RS485CTRL_API short ErroMessage()
{

	if(!ReadHoldRegister(0620,1))//实际值，对应寄存器地址40005
		return -1;
	if (NULL == recvData)
		return -1;
	return *recvData;
}
RS485CTRL_API bool  CommunicationReady()
{
	//ComFlag = FALSE;
	return ComFlag;

}

void InsertMap()
{
	valueMap.insert(pair<short,string>(8976,"过流"));
	valueMap.insert(pair<short,string>(9089,"IGBT 过载"));
	valueMap.insert(pair<short,string>(26241,"EFB 通讯断开"));
	valueMap.insert(pair<short,string>(29456,"超速"));


}
RS485CTRL_API LPCTSTR GetErroExplain(short code)
{
	InsertMap();
	map<short,string>::iterator it;

	LPCTSTR str = L"";
	for (it=valueMap.begin();it!=valueMap.end();it++)
	{

		if (it->first == code)
		{
			str = (LPCTSTR)(it->second.c_str()) ;
		}		
	}
	return str;
}
