import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { LayoutDashboard, Users, Building2, LogOut, ShieldCheck, UserPlus, Heart, Settings } from 'lucide-react';

export default function DashboardLayout({ setAuth }) {
    const navigate = useNavigate();
    const location = useLocation();
    
    const handleLogout = () => { 
        localStorage.removeItem('admin_token'); 
        setAuth(false); 
        navigate('/login'); 
    };
    
    const navItem = (path, icon, label) => {
        const isActive = location.pathname === path;
        return (
            <Link to={path} className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-300 font-medium ${isActive ? 'bg-gradient-to-r from-teal-500 to-teal-700 text-white shadow-lg' : 'text-gray-400 hover:bg-gray-800 hover:text-white'}`}>
                {icon} {label}
            </Link>
        );
    };

    return (
        <div className="flex h-screen bg-[#f8fafc] overflow-hidden">
            {/* القائمة الجانبية الداكنة والفخمة */}
            <aside className="w-72 bg-[#0f172a] text-white flex flex-col shadow-2xl z-20">
                <div className="p-8 text-center border-b border-gray-800">
                    <div className="w-16 h-16 bg-gradient-to-br from-teal-400 to-teal-600 rounded-2xl mx-auto flex items-center justify-center shadow-lg mb-4 transform rotate-3">
                        <ShieldCheck className="w-10 h-10 text-white transform -rotate-3" />
                    </div>
                    <h1 className="text-2xl font-bold tracking-wide">بوابة عون</h1>
                    <p className="text-xs text-teal-400 mt-1 font-bold">الإدارة المركزية | Control Panel</p>
                </div>
                
                <nav className="flex-1 p-4 space-y-2 overflow-y-auto">
                    <p className="text-xs text-gray-500 font-bold px-4 mb-2 mt-4 uppercase">الرئيسية</p>
                    {navItem('/', <LayoutDashboard size={20}/>, 'لوحة المؤشرات')}
                    
                    <p className="text-xs text-gray-500 font-bold px-4 mb-2 mt-6 uppercase">الإدارة والتحكم</p>
                    {navItem('/users', <Users size={20}/>, 'إدارة المتبرعين')}
                    {navItem('/charities', <Building2 size={20}/>, 'مراجعة الجمعيات')}
                    {navItem('/cases', <Heart size={20}/>, 'الحالات والتبرعات')}
                    
                    <p className="text-xs text-gray-500 font-bold px-4 mb-2 mt-6 uppercase">النظام</p>
                    {navItem('/admins', <UserPlus size={20}/>, 'مديري النظام')}
                    {navItem('/settings', <Settings size={20}/>, 'إعدادات المنصة')}
                </nav>

                <div className="p-6 border-t border-gray-800">
                    <button onClick={handleLogout} className="flex items-center justify-center gap-2 w-full px-4 py-3 bg-red-500/10 text-red-500 hover:bg-red-500 hover:text-white rounded-xl transition-all font-bold">
                        <LogOut size={20} /> تسجيل خروج آمن
                    </button>
                </div>
            </aside>

            {/* المحتوى الرئيسي ذو الخلفية الفاتحة */}
            <main className="flex-1 flex flex-col h-full overflow-hidden relative">
                {/* خلفية جمالية علوية */}
                <div className="absolute top-0 left-0 w-full h-64 bg-gradient-to-b from-teal-600/10 to-transparent pointer-events-none z-0"></div>
                
                <header className="glass-card shadow-sm p-6 flex justify-between items-center z-10 m-6 mb-0 rounded-2xl">
                    <div>
                        <h2 className="text-2xl font-black text-gray-800">مرحباً بك في مركز القيادة 🚀</h2>
                        <p className="text-sm text-gray-500">مراقبة حية لجميع عمليات منصة عون الخيرية</p>
                    </div>
                    <div className="flex items-center gap-4 bg-white px-4 py-2 rounded-full shadow-sm border">
                        <div className="flex flex-col text-left">
                            <span className="font-bold text-gray-800 text-sm">مدير النظام</span>
                            <span className="text-xs text-teal-600 font-bold">Super Admin</span>
                        </div>
                        <div className="w-10 h-10 bg-teal-600 text-white rounded-full flex items-center justify-center font-bold shadow-md">
                            AD
                        </div>
                    </div>
                </header>

                <div className="p-6 overflow-y-auto h-full z-10">
                    <Outlet />
                </div>
            </main>
        </div>
    );
}