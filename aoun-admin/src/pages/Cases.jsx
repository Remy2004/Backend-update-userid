/* eslint-disable no-unused-vars */
import { useState, useEffect } from 'react';
import { Trash2, AlertCircle, Eye, Search, Heart, Activity } from 'lucide-react';
import api from '../api/axios';

export default function Cases() {
    const [cases, setCases] = useState([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState("");

    useEffect(() => {
        const fetchCases = async () => {
            try {
                const res = await api.get('/Admin/cases'); // تأكد من الـ Endpoint في الباك إند
                setCases(res.data);
            } catch (err) { console.error(err); }
            finally { setLoading(false); }
        };
        fetchCases();
    }, []);

    const deleteCase = async (id) => {
        if (window.confirm("Are you sure you want to delete this case? This action is irreversible.")) {
            try {
                await api.delete(`/Admin/cases/${id}`);
                setCases(prev => prev.filter(c => c.id !== id));
            } catch (err) { alert("Delete failed"); }
        }
    };

    const filteredCases = cases.filter(c => c.title.toLowerCase().includes(search.toLowerCase()));

    return (
        <div className="space-y-10 animate-in fade-in duration-700">
            <header className="flex justify-between items-center">
                <div>
                    <h1 className="text-4xl font-black text-white italic">Active Cases <span className="text-brand-accent">.</span></h1>
                    <p className="text-slate-400 font-medium">Monitor and moderate ongoing donation campaigns</p>
                </div>
                <div className="relative w-72 group">
                    <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 group-focus-within:text-brand-accent transition-colors" size={18}/>
                    <input 
                        onChange={(e) => setSearch(e.target.value)}
                        type="text" placeholder="Search cases..." 
                        className="w-full bg-white/5 border border-white/10 rounded-2xl py-3 pl-12 pr-4 text-white focus:border-brand-accent/50 outline-none"
                    />
                </div>
            </header>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredCases.map(item => (
                    <div key={item.id} className="glass-card p-6 rounded-[2rem] group hover:border-brand-accent/30 transition-all">
                        <div className="flex justify-between items-start mb-4">
                            <span className="px-3 py-1 bg-brand-accent/10 text-brand-accent text-[10px] font-black uppercase rounded-lg border border-brand-accent/20">
                                {item.category || 'General'}
                            </span>
                            <button onClick={() => deleteCase(item.id)} className="text-slate-500 hover:text-rose-500 transition-colors">
                                <Trash2 size={20}/>
                            </button>
                        </div>
                        <h3 className="text-xl font-bold text-white mb-2 line-clamp-1">{item.title}</h3>
                        <p className="text-slate-400 text-sm mb-6 line-clamp-2 leading-relaxed">{item.description}</p>
                        
                        <div className="space-y-3">
                            <div className="flex justify-between text-xs font-bold uppercase tracking-wider">
                                <span className="text-slate-500 text-right">Progress</span>
                                <span className="text-brand-accent">{Math.round((item.collectedAmount / item.requiredAmount) * 100)}%</span>
                            </div>
                            <div className="h-2 w-full bg-white/5 rounded-full overflow-hidden">
                                <div 
                                    className="h-full bg-gradient-to-r from-brand-accent to-blue-500 transition-all duration-1000"
                                    style={{ width: `${(item.collectedAmount / item.requiredAmount) * 100}%` }}
                                ></div>
                            </div>
                            <div className="flex justify-between items-end mt-4">
                                <div>
                                    <p className="text-[10px] text-slate-500 font-black uppercase">Collected</p>
                                    <p className="text-white font-black">${item.collectedAmount.toLocaleString()}</p>
                                </div>
                                <div className="text-right">
                                    <p className="text-[10px] text-slate-500 font-black uppercase">Goal</p>
                                    <p className="text-slate-300 font-black">${item.requiredAmount.toLocaleString()}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}