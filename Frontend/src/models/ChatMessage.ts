import { ChatRole } from "./ChatRole";
import { Dayjs } from "dayjs";

export type ChatMessage = {
  id: string;
  role: ChatRole;
  content: string;
  createdOn: Dayjs;
};
