#ifdef RS485MODBUS_EXPORTS
#define RS485MODBUS_API __declspec(dllexport)
#else
#define RS485MODBUS_API __declspec(dllimport)
#endif

struct SendSeal16
{
	unsigned char functionCode;	//功能码
	unsigned short jcqAddr;		//起始地址/输出地址/寄存器地址
	unsigned short counts;		//线圈数/数量/寄存器数/输出值/寄存器值
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

	unsigned char recvFuncCode; //接收的功能码
	unsigned char recvBytes;	//字节数
	unsigned char* state;		//线圈状态/输出状态
	unsigned short* data;		//寄存器值/输入寄存器
	unsigned int recvDataLen;	//保存state或者data的数据个数
	unsigned short recvOutAddr;	//输出地址/寄存器地址/起始地址
	unsigned short recvData;	//输出值/寄存器值/输出数量/寄存器数量

	bool bError;				//标记接收的响应是否异常

	unsigned char errorFuncCode;//异常功能码
	unsigned char errorCode;	//异常码  01-非法功能 02-非法数据地址 03-非法数据值 04-从站设备故障 05-确认 06-从属设备忙 08-存储奇偶性差错 0A-不可用网关路径 0B-网关目标设备响应失败
};

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	RS485MODBUS_API bool GetSealCommand(SendSeal16* m_seal);
	RS485MODBUS_API void AnalysisRecvSeal(unsigned char* recv,unsigned int recvLen,SendSeal16* m_seal);

	//m_seal-协议结构体，functionCode-功能码，jcqAddr-寄存器地址，counts-数量，bytes-字节数，outData-输出数据，jcqData-寄存器值，dataLen-输出数据或寄存器值数据个数，bHighBefore-高位在前，bPolynomail-CRC码校验，polynomail-CRC校验码
	RS485MODBUS_API bool InitSeal(SendSeal16* m_seal,unsigned char functionCode,unsigned short jcqAddr,unsigned short counts,unsigned char bytes = 0x00,unsigned char* outData = NULL,unsigned short* jcqData = NULL,unsigned int dataLen = 0,bool bHighBefore = false,bool bPolynomail = true,unsigned short polynomail = 0xa001);

	RS485MODBUS_API unsigned int GetSealLen(SendSeal16* m_seal);
	RS485MODBUS_API void SetHighBefore(SendSeal16* m_seal,bool bBefore);
	RS485MODBUS_API bool GetHighBefore(SendSeal16* m_seal);
	RS485MODBUS_API void SetBPolyNomail(SendSeal16* m_seal,bool bPoly);
	RS485MODBUS_API void SetFjAddr(char addr);
	RS485MODBUS_API bool GetBPolyNomail(SendSeal16* m_seal);
	RS485MODBUS_API void SetNomail(SendSeal16* m_seal,unsigned short bNomail);
	RS485MODBUS_API unsigned short GetNomail(SendSeal16* m_seal);
	RS485MODBUS_API void FreeSeal(SendSeal16* m_seal);
#ifdef __cplusplus
};
#endif