﻿select Account.Ten,Account.Id from Account,Friend where Friend.Id_M=6 and Friend.Id_N=Account.Id or Friend.Id_N=6 and Friend.Id_M=Account.Id