import { AreaChart, Area, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid, PieChart, Pie, Cell } from 'recharts';
import { Activity, ShieldCheck, Users, TrendingUp, AlertCircle, CheckCircle, Bell, BellDot } from 'lucide-react';

const areaData = [{n:'Sat', v:4000}, {n:'Sun', v:7000}, {n:'Mon', v:5000}, {n:'Tue', v:9000}, {n:'Wed', v:11000}, {n:'Thu', v:8000}];
const pieData = [
    { name: 'Health', value: 400, color: '#38bdf8' },
    { name: 'Education', value: 300, color: '#818cf8' },
    { name: 'Food', value: 300, color: '#fb7185' },
];
export function NotificationBell({ count }) {
    return (
        <div className="relative cursor-pointer group">
            <div className="p-3 bg-white/5 rounded-2xl border border-white/10 group-hover:bg-brand-accent/10 transition-all">
                {count > 0 ? <BellDot className="text-brand-accent" /> : <Bell className="text-slate-400" />}
            </div>
            {count > 0 && (
                <span className="absolute -top-1 -right-1 w-5 h-5 bg-rose-500 text-white text-[10px] font-black flex items-center justify-center rounded-full border-2 border-[#0b1120]">
                    {count}
                </span>
            )}
        </div>
    );
}
export default function Dashboard() {
    return (
        <div className="space-y-10 animate-in fade-in duration-1000">
            {/* Upper Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                <MiniStat title="Platform Users" value="12,543" icon={<Users/>} color="text-sky-400" />
                <MiniStat title="Success Rate" value="94.2%" icon={<CheckCircle/>} color="text-emerald-400" />
                <MiniStat title="Reports" value="12" icon={<AlertCircle/>} color="text-rose-400" />
                <MiniStat title="Active Nodes" value="08" icon={<Activity/>} color="text-amber-400" />
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Main Graph */}
                <div className="lg:col-span-2 glass-card p-8 rounded-[2.5rem]">
                    <div className="flex justify-between items-center mb-8">
                        <h3 className="text-xl font-bold flex items-center gap-2"><TrendingUp className="text-brand-accent"/> Global Donation Flow</h3>
                        <span className="text-xs font-black text-slate-500 uppercase tracking-widest bg-white/5 px-4 py-1 rounded-full">Last 7 Days</span>
                    </div>
                    <div className="h-[350px]">
                        <ResponsiveContainer width="100%" height="100%">
                            <AreaChart data={areaData}>
                                <defs>
                                    <linearGradient id="colorV" x1="0" y1="0" x2="0" y2="1">
                                        <stop offset="5%" stopColor="#38bdf8" stopOpacity={0.3}/>
                                        <stop offset="95%" stopColor="#38bdf8" stopOpacity={0}/>
                                    </linearGradient>
                                </defs>
                                <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#334155" opacity={0.1}/>
                                <XAxis dataKey="n" stroke="#475569" fontSize={12} tickLine={false} axisLine={false} />
                                <Tooltip contentStyle={{backgroundColor: '#0b1120', border: 'none', borderRadius: '12px'}} />
                                <Area type="monotone" dataKey="v" stroke="#38bdf8" strokeWidth={4} fill="url(#colorV)" />
                            </AreaChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                {/* Distribution */}
                <div className="glass-card p-8 rounded-[2.5rem] flex flex-col items-center justify-center">
                    <h3 className="text-xl font-bold mb-6">Categories</h3>
                    <div className="h-[250px] w-full">
                        <ResponsiveContainer width="100%" height="100%">
                            <PieChart>
                                <Pie data={pieData} innerRadius={60} outerRadius={80} paddingAngle={10} dataKey="value">
                                    {pieData.map((entry, index) => <Cell key={`cell-${index}`} fill={entry.color} />)}
                                </Pie>
                                <Tooltip />
                            </PieChart>
                        </ResponsiveContainer>
                    </div>
                    <div className="grid grid-cols-2 gap-4 w-full mt-4">
                        {pieData.map(item => (
                            <div key={item.name} className="flex items-center gap-2">
                                <div className="w-3 h-3 rounded-full" style={{backgroundColor: item.color}}></div>
                                <span className="text-xs font-bold text-slate-400">{item.name}</span>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

            {/* Bottom Section: Recent Logs */}
            <div className="glass-card p-8 rounded-[2.5rem]">
                <h3 className="text-xl font-bold mb-6 flex items-center gap-3"><ShieldCheck className="text-emerald-500"/> System Security Logs</h3>
                <div className="space-y-4">
                    {[
                        { action: "Admin Login", user: "admin@test.com", time: "2 mins ago", status: "Successful" },
                        { action: "Charity Deleted", user: "system_core", time: "1 hour ago", status: "Warning" },
                        { action: "New Case Approved", user: "admin@test.com", time: "3 hours ago", status: "Action" },
                    ].map((log, i) => (
                        <div key={i} className="flex justify-between items-center p-4 bg-white/[0.02] border border-white/5 rounded-2xl">
                            <div className="flex items-center gap-4">
                                <div className={`w-2 h-2 rounded-full ${log.status === 'Successful' ? 'bg-emerald-500' : 'bg-amber-500'}`}></div>
                                <div>
                                    <p className="text-sm font-bold text-white">{log.action}</p>
                                    <p className="text-xs text-slate-500">{log.user}</p>
                                </div>
                            </div>
                            <span className="text-xs text-slate-600 font-mono italic">{log.time}</span>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

function MiniStat({ title, value, icon, color }) {
    return (
        <div className="glass-card p-6 rounded-[2rem] flex items-center gap-5 border-l-4 border-l-transparent hover:border-l-brand-accent transition-all">
            <div className={`p-4 rounded-2xl bg-white/5 ${color}`}>{icon}</div>
            <div>
                <p className="text-[10px] font-black text-slate-500 uppercase tracking-widest">{title}</p>
                <h4 className="text-2xl font-black text-white leading-none mt-1">{value}</h4>
            </div>
        </div>
    );
}