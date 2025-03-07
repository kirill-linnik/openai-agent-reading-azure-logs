import { ChatRole } from "./ChatRole";

export type ChatMessage = {
  role: ChatRole;
  content: string;
};
