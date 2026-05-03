import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Building2, ClipboardList, LogOut, Zap } from 'lucide-react';

export default function Sidebar() {
    const handleLogout = () => {
    localStorage.removeItem('token'); 
    window.location.href = '/login';
};
    const menu = [
        { path: '/admin', icon: <LayoutDashboard />, label: 'Analytics' },
        { path: '/admin/charities', icon: <Building2 />, label: 'Charities' },
        { path: '/admin/cases', icon: <ClipboardList />, label: 'Global Cases' },
    ];

    return (
        <aside className="w-72 bg-brand-dark h-screen sticky top-0 border-r border-white/5 flex flex-col">
            <div className="p-10 flex items-center gap-3">
                <div className="w-10 h-10 bg-brand-accent rounded-xl flex items-center justify-center shadow-lg shadow-brand-accent/20">
                    <Zap className="text-brand-dark" fill="currentColor" />
                </div>
                <span className="text-2xl font-black tracking-tighter">AOUN <span className="text-brand-accent">.</span></span>
            </div>
            
            <nav className="flex-1 px-6 space-y-3">
                {menu.map(item => (
                    <NavLink key={item.path} to={item.path} end className={({ isActive }) => 
                        `flex items-center gap-4 px-5 py-4 rounded-2xl font-bold transition-all ${
                            isActive ? 'bg-brand-accent text-brand-dark shadow-lg shadow-brand-accent/10' : 'text-gray-500 hover:text-white hover:bg-white/5'
                        }`
                    }>
                        {item.icon} <span>{item.label}</span>
                    </NavLink>
                ))}
            </nav>

            <div className="p-8 border-t border-white/5">
<button 
    onClick={handleLogout}
    className="flex items-center gap-4 px-5 py-4 w-full text-red-400 hover:bg-red-400/10 rounded-2xl font-bold transition-all"
>
    <LogOut /> Logout
</button>
            </div>
        </aside>
    );
}