<template>
  <div class="flex flex-col w-full h-full"
    :class="'bg-white dark:bg-slate-900'">
    <div ref="messagesContainer"
      class="flex-1 overflow-y-auto space-y-2 mb-3 custom-scrollbar px-1">
      <div v-if="messages.length === 0"
        class="flex flex-col items-center justify-center h-48 space-y-3">
        <MessageSquareIcon class="w-8 h-8 opacity-20"
          :class="'text-slate-400 dark:text-slate-500'" />
        <p class="text-xs font-bold uppercase tracking-widest"
          :class="'text-slate-400 dark:text-slate-600'">
          No activity yet
        </p>
      </div>
      <div v-for="(msg, index) in messages" :key="msg.id">
        <div v-if="showDateSeparator(index)" class="flex items-center gap-3 my-3">
          <div class="flex-1 h-px" :class="'bg-slate-100 dark:bg-slate-800'"></div>
          <span class="text-[9px] font-black uppercase tracking-widest"
            :class="'text-slate-400 dark:text-slate-600'">
            {{ formatDateSeparator(msg.timestamp) }}
          </span>
          <div class="flex-1 h-px" :class="'bg-slate-100 dark:bg-slate-800'"></div>
        </div>
        <div :class="isOwn(msg) ? 'flex justify-end' : 'flex justify-start'">

          <div v-if="msg.isInternal"
            class="w-full max-w-sm rounded-xl border border-amber-200 dark:border-amber-800/30 bg-amber-50 dark:bg-amber-900/20 px-3 py-2 relative mt-2">
            <span class="absolute -top-2 right-2 bg-amber-500 dark:bg-amber-600 text-[8px] font-black text-black px-2 py-0.5 rounded-full uppercase tracking-wider">
              Internal Note
            </span>
            <p class="text-[10px] font-semibold text-amber-700/70 dark:text-amber-400/70 mb-1">{{ msg.author }}</p>
            <p v-if="isTextVisible(msg)" class="text-sm text-amber-800 dark:text-amber-200">{{ msg.message }}</p>
            <div v-if="msg.attachments?.length" class="mt-2 space-y-1.5">
              <template v-for="adj in msg.attachments" :key="adj.id">
                <a v-if="isImage(adj)" :href="resolveUrl(adj.url)" target="_blank" class="block">
                  <img :src="resolveUrl(adj.url)" :alt="adj.originalName"
                    class="max-w-[220px] rounded-lg border border-amber-200 dark:border-amber-800/30 object-cover cursor-pointer hover:opacity-90 transition-opacity" />
                </a>
                <a v-else :href="resolveUrl(adj.url)" target="_blank"
                  class="flex items-center gap-2 text-xs text-amber-700 dark:text-amber-300 hover:underline">
                  <component :is="getFileIcon(adj)" class="w-3.5 h-3.5 flex-shrink-0" />
                  <span class="truncate max-w-[180px]">{{ adj.originalName }}</span>
                </a>
              </template>
            </div>
            <p class="text-[9px] text-amber-600/60 dark:text-amber-500/60 mt-1 font-mono">{{ formatTime(msg.timestamp) }}</p>
          </div>

          <div v-else-if="isOwn(msg)" class="flex flex-col items-end gap-0.5 max-w-[85%] md:max-w-xs">
            <div class="px-3 md:px-4 py-2 rounded-2xl rounded-tr-sm bg-blue-600 text-white shadow-sm">
              <p v-if="isTextVisible(msg)" class="text-sm leading-relaxed">{{ msg.message }}</p>
              <div v-if="msg.attachments?.length" class="mt-2 space-y-1.5" :class="{ 'mt-0': !isTextVisible(msg) }">
                <template v-for="adj in msg.attachments" :key="adj.id">
                  <a v-if="isImage(adj)" :href="resolveUrl(adj.url)" target="_blank" class="block">
                    <img :src="resolveUrl(adj.url)" :alt="adj.originalName"
                      class="max-w-full rounded-xl object-cover cursor-pointer hover:opacity-90 transition-opacity" />
                  </a>
                  <a v-else :href="resolveUrl(adj.url)" target="_blank"
                    class="flex items-center gap-2 text-xs text-blue-100 hover:text-white hover:underline">
                    <component :is="getFileIcon(adj)" class="w-3.5 h-3.5 flex-shrink-0" />
                    <span class="truncate max-w-[140px] md:max-w-[180px]">{{ adj.originalName }}</span>
                  </a>
                </template>
              </div>
            </div>
            <p class="text-[9px] font-mono px-1" :class="'text-slate-400 dark:text-slate-600'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>

          <div v-else class="flex flex-col items-start gap-0.5 max-w-[85%] md:max-w-xs">
            <p class="text-[10px] font-semibold px-1" :class="'text-slate-400 dark:text-slate-500'">
              {{ msg.author }}
              <span class="uppercase tracking-widest text-[8px] ml-1 px-1.5 py-0.5 rounded-full"
                :class="roleBadgeClass(msg.authorRole)">
                {{ roleLabel(msg.authorRole) }}
              </span>
            </p>
            <div class="px-3 md:px-4 py-2 rounded-2xl rounded-tl-sm border"
              :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white shadow-sm'">
              <p v-if="isTextVisible(msg)" class="text-sm leading-relaxed">{{ msg.message }}</p>
              <div v-if="msg.attachments?.length" class="mt-2 space-y-1.5" :class="{ 'mt-0': !isTextVisible(msg) }">
                <template v-for="adj in msg.attachments" :key="adj.id">
                  <a v-if="isImage(adj)" :href="resolveUrl(adj.url)" target="_blank" class="block">
                    <img :src="resolveUrl(adj.url)" :alt="adj.originalName"
                      class="max-w-full rounded-xl object-cover cursor-pointer hover:opacity-90 transition-opacity" />
                  </a>
                  <a v-else :href="resolveUrl(adj.url)" target="_blank"
                    class="flex items-center gap-2 text-xs text-blue-500 hover:underline">
                    <component :is="getFileIcon(adj)" class="w-3.5 h-3.5 flex-shrink-0" />
                    <span class="truncate max-w-[140px] md:max-w-[180px]">{{ adj.originalName }}</span>
                  </a>
                </template>
              </div>
            </div>
            <p class="text-[9px] font-mono px-1" :class="'text-slate-400 dark:text-slate-600'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>

        </div>
      </div>
      <div v-if="someoneIsTyping" class="flex justify-start">
        <div class="px-4 py-3 rounded-2xl rounded-tl-sm border"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700'">
          <div class="flex gap-1 items-center">
            <span v-for="i in 3" :key="i"
              class="w-1.5 h-1.5 rounded-full bg-slate-400 animate-bounce"
              :style="`animation-delay: ${(i-1)*0.2}s`"></span>
          </div>
        </div>
      </div>
    </div>

    <div class="space-y-2 flex-shrink-0 border-t pt-3"
      :class="'border-slate-100 dark:border-slate-800'">

      <div v-if="authStore.isAdmin || authStore.isOperator"
        class="flex items-center gap-2 px-1">
        <button @click="isInternalMode = !isInternalMode" class="flex items-center gap-2 group">
          <div class="w-7 h-3.5 rounded-full relative transition-all"
            :class="isInternalMode ? 'bg-amber-500 dark:bg-amber-600' : 'bg-slate-200 dark:bg-slate-700'">
            <div class="absolute top-0.5 w-2.5 h-2.5 rounded-full bg-white dark:bg-slate-900 transition-all shadow-sm"
              :class="isInternalMode ? 'left-[calc(100%-0.75rem)]' : 'left-0.5'"></div>
          </div>
          <span class="text-[10px] font-black uppercase tracking-widest transition-colors"
            :class="isInternalMode ? 'text-amber-600 dark:text-amber-400' : 'text-slate-400 dark:text-slate-600'">
            Internal Note
          </span>
        </button>
      </div>

      <div v-if="pendingFiles.length > 0" class="flex flex-wrap gap-2 px-1">
        <div v-for="(f, i) in pendingFiles" :key="i"
          class="flex items-center gap-1.5 text-xs px-2 py-1 rounded-lg border"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-600 dark:text-slate-300'">
          <PaperclipIcon class="w-3 h-3 flex-shrink-0" />
          <span class="max-w-[100px] truncate">{{ f.name }}</span>
          <button @click="removeFile(i)" class="text-slate-400 hover:text-rose-400 transition-colors ml-0.5">
            <XIcon class="w-3 h-3" />
          </button>
        </div>
      </div>

      <div class="flex gap-2 items-end">
        <textarea
          ref="inputRef"
          v-model="newMessage"
          @keydown.enter.exact.prevent="sendMessage"
          @keydown.enter.shift.exact="newMessage += '\n'"
          @input="autoResize(); notifyTyping()"
          rows="1"
          :placeholder="isInternalMode ? 'Internal note...' : 'Write a message...'"
          class="flex-1 border px-4 py-3 rounded-xl text-sm outline-none transition-all font-medium resize-none"
          :class="'bg-slate-50 dark:bg-slate-800 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white placeholder-slate-400 focus:border-blue-400 dark:focus:border-blue-500'"
          style="max-height: 120px; overflow-y: auto;" />

        <div class="flex items-center gap-1 mb-1">
          <button @click="$refs.fileInputRef.click()"
            class="p-2 rounded-xl transition-all hover:bg-slate-100 dark:hover:bg-slate-800 group"
            title="Attach File">
            <PaperclipIcon class="w-5 h-5 text-slate-400 group-hover:text-blue-500" />
          </button>
          
          <button @click="sendMessage"
            :disabled="(!newMessage.trim() && !pendingFiles.length) || sending"
            class="p-2.5 rounded-xl bg-blue-600 text-white shadow-md shadow-blue-500/20 hover:bg-blue-700 active:scale-95 disabled:opacity-50 disabled:active:scale-100 transition-all">
            <SendIcon v-if="!sending" class="w-5 h-5" />
            <Loader2Icon v-else class="w-5 h-5 animate-spin" />
          </button>
        </div>
      </div>
      
      <input ref="fileInputRef" type="file" multiple class="hidden"
        @change="onFileSelected" />
    </div>

    <div v-if="uploadProgress > 0" class="h-1 bg-slate-100 dark:bg-slate-800 rounded-full overflow-hidden mt-2">
      <div class="h-full bg-blue-500 transition-all duration-300" :style="{ width: `${uploadProgress}%` }"></div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, nextTick, watch } from 'vue';
import { 
  MessageSquareIcon, 
  PaperclipIcon, 
  SendIcon, 
  XIcon, 
  FileIcon, 
  FileTextIcon, 
  FileSpreadsheetIcon, 
  ImageIcon,
  Loader2Icon
} from 'lucide-vue-next';
import { useAuthStore } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';

const props = defineProps({
  ticketId: { type: String, required: true }
});

const ticketsStore = useTicketsStore();
const authStore    = useAuthStore();
const API_URL      = import.meta.env.VITE_API_URL || 'http://localhost:5134/api';
const BACKEND_BASE = API_URL.replace('/api', '');

const messages          = ref([]);
const newMessage        = ref('');
const isInternalMode    = ref(false);
const sending           = ref(false);
const someoneIsTyping   = ref(false);
const uploadProgress    = ref(0);
const pendingFiles      = ref([]);
const messagesContainer = ref(null);
const inputRef          = ref(null);
const fileInputRef      = ref(null);

let connection      = null;
let typingTimer     = null;
const PLACEHOLDER_TEXT = '---ATTACHMENT_ONLY---';

const resolveUrl = (url) => {
  if (!url) return '';
  if (url.startsWith('http')) return url;
  return `${BACKEND_BASE}${url.startsWith('/') ? '' : '/'}${url}`;
};

const isTextVisible = (msg) => {
  if (!msg.message || msg.message === PLACEHOLDER_TEXT) return false;
  return true;
};

const IMAGE_EXTENSIONS = ['jpg', 'jpeg', 'png', 'gif', 'webp', 'svg', 'bmp'];

const getFileExt = (adj) => {
  const name = adj.originalName ?? adj.url ?? '';
  return name.split('.').pop()?.toLowerCase() ?? '';
};

const isImage = (adj) => IMAGE_EXTENSIONS.includes(getFileExt(adj));

const getFileIcon = (adj) => {
  const ext = getFileExt(adj);
  if (['pdf'].includes(ext))                         return FileTextIcon;
  if (['xls', 'xlsx', 'csv'].includes(ext))          return FileSpreadsheetIcon;
  if (['doc', 'docx', 'txt'].includes(ext))          return FileTextIcon;
  if (IMAGE_EXTENSIONS.includes(ext))                return ImageIcon;
  return FileIcon;
};

const parseDate = (timestamp) => {
  if (!timestamp) return null;
  const str = timestamp.toString();
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? null : d;
};

const formatTime = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  return new Intl.DateTimeFormat('en-US', {
    hour: '2-digit', minute: '2-digit'
  }).format(date);
};

const formatDateSeparator = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  const now       = new Date();
  const today     = new Date(now.toLocaleDateString('en-CA'));
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);
  const dateDay   = new Date(date.toLocaleDateString('en-CA'));
  
  if (dateDay.getTime() === today.getTime())     return 'Today';
  if (dateDay.getTime() === yesterday.getTime()) return 'Yesterday';
  
  return new Intl.DateTimeFormat('en-US', {
    day: '2-digit', month: 'long', year: 'numeric'
  }).format(date);
};

const showDateSeparator = (index) => {
  if (index === 0) return true;
  const prev = parseDate(messages.value[index - 1]?.timestamp);
  const curr = parseDate(messages.value[index]?.timestamp);
  if (!prev || !curr) return false;
  return prev.toLocaleDateString('en-CA') !== curr.toLocaleDateString('en-CA');
};

const roleLabel = (r) => {
  if (r === null || r === undefined) return '';
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  return n === 2 ? 'Admin' : n === 1 ? 'Operator' : 'User';
};

const roleBadgeClass = (r) => {
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  if (n === 2 || r === 'Administrator') return 'bg-purple-500/10 text-purple-400';
  if (n === 1 || r === 'Operator') return 'bg-emerald-500/10 text-emerald-400';
  return 'bg-slate-500/10 text-slate-400';
};

const isOwn = (msg) => {
  const userId  = authStore.user?.id?.toString().toLowerCase();
  const authorId = msg.authorId?.toString().toLowerCase();
  return userId && authorId && userId === authorId;
};

const mapComment = (c) => ({
  id:        c.id,
  authorId:  c.authorId,
  author:    c.author || 'System',
  authorRole: c.authorRole ?? c.role ?? null,
  message:   c.message,
  isInternal: c.isInternal,
  timestamp: c.createdAt ?? null,
  attachments: c.attachments ?? []
});

const scrollToBottom = (smooth = false) => {
  nextTick(() => {
    if (!messagesContainer.value) return;
    messagesContainer.value.scrollTo({
      top: messagesContainer.value.scrollHeight,
      behavior: smooth ? 'smooth' : 'instant'
    });
  });
};

const autoResize = () => {
  const el = inputRef.value;
  if (!el) return;
  el.style.height = 'auto';
  el.style.height = Math.min(el.scrollHeight, 120) + 'px';
};

const resetInput = () => {
  newMessage.value = '';
  nextTick(() => {
    if (inputRef.value) {
      inputRef.value.style.height = 'auto';
      inputRef.value.focus();
    }
  });
};

const onFileSelected = (e) => {
  const files = Array.from(e.target.files ?? []);
  pendingFiles.value = [...pendingFiles.value, ...files];
  if (fileInputRef.value) fileInputRef.value.value = '';
};

const removeFile = (i) => {
  pendingFiles.value = pendingFiles.value.filter((_, idx) => idx !== i);
};

const uploadFiles = async (commentId) => {
  if (!pendingFiles.value.length) return;
  const total = pendingFiles.value.length;
  for (let i = 0; i < total; i++) {
    const form = new FormData();
    form.append('file', pendingFiles.value[i]);
    await axios.post(`${API_URL}/files/comment/${commentId}`, form, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    uploadProgress.value = Math.round(((i + 1) / total) * 100);
  }
  pendingFiles.value   = [];
  uploadProgress.value = 0;
};

const getToken = () => localStorage.getItem('token') ?? '';

const connectSignalR = async () => {
  if (!props.ticketId) return;
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${BACKEND_BASE}/hubs/tickets`, { accessTokenFactory: () => getToken() })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on('NewMessage', (comment) => {
    const msg = mapComment(comment);
    if (!messages.value.find(m => m.id === msg.id)) {
      messages.value.push(msg);
      scrollToBottom(true);
    }
  });

  connection.on('UserTyping', () => {
    someoneIsTyping.value = true;
    clearTimeout(typingTimer);
    typingTimer = setTimeout(() => { someoneIsTyping.value = false; }, 2500);
  });

  try {
    await connection.start();
    await connection.invoke('JoinTicket', props.ticketId);
  } catch (e) {
    console.warn('[SignalR] Not available, falling back to REST:', e.message);
  }
};

const disconnectSignalR = async () => {
  if (connection) {
    try {
      if (props.ticketId) await connection.invoke('LeaveTicket', props.ticketId);
      await connection.stop();
    } catch { }
    connection = null;
  }
};

const notifyTyping = async () => {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    try { await connection.invoke('NotifyTyping', props.ticketId); } catch { }
  }
};

const loadMessages = async (id) => {
  if (!id) return;
  await ticketsStore.fetchComments(id);
  messages.value = ticketsStore.comments.map(mapComment);
  scrollToBottom(false);
  nextTick(() => inputRef.value?.focus());
};

watch(() => props.ticketId, async (id) => {
  await disconnectSignalR();
  messages.value = [];
  if (id) {
    await loadMessages(id);
    await connectSignalR();
  }
}, { immediate: true });

const sendMessage = async () => {
  const text     = newMessage.value.trim();
  const isInternal = isInternalMode.value;
  if ((!text && pendingFiles.value.length === 0) || !props.ticketId || sending.value) return;

  sending.value = true;
  resetInput();

  try {
    if (connection?.state === signalR.HubConnectionState.Connected && !pendingFiles.value.length) {
      await connection.invoke('SendMessage', props.ticketId, text, isInternal);
    } else {
      const saved = await ticketsStore.addComment(props.ticketId, {
        message: text || PLACEHOLDER_TEXT,
        isInternal
      });
      if (pendingFiles.value.length > 0 && saved?.id) {
        await uploadFiles(saved.id);
      }
      await loadMessages(props.ticketId);
    }
  } catch (e) {
    console.error(e);
    newMessage.value = text;
  } finally {
    sending.value = false;
  }
};

onMounted(() => { nextTick(() => inputRef.value?.focus()); });
onUnmounted(disconnectSignalR);
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.15); border-radius: 10px; }
</style>
