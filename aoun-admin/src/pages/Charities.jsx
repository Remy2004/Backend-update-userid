import { useState, useMemo, useEffect, useCallback } from "react";
import {
  Check,
  X,
  Search,
  FileText,
  Loader2,
  ShieldCheck,
  Hash,
  Mail,
} from "lucide-react";
import api from "../api/axios";

export default function Charities() {
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [charities, setCharities] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchCharities = useCallback(async () => {
    try {
      setLoading(true);
      const res = await api.get("/Admin/charities");
      setCharities(res.data);
    } catch (error) {
      console.error("Fetch error:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    fetchCharities();
  }, [fetchCharities]);

  const handleAction = async (id, action) => {
    try {
      await api.put(`/Admin/verify-charity/${id}`, { status: action });
      setCharities((prev) =>
        prev.map((c) =>
          c.id === id ? { ...c, status: action === "approve" ? 1 : 2 } : c,
        ),
      );
    } catch (error) {
      alert(
        "Action failed: " +
          (error.response?.data?.message || "Check API connection"),
      );
    }
  };

  const filteredData = useMemo(() => {
    return charities.filter((c) => {
      const matchesSearch =
        c.charityName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        c.licenseNumber?.toLowerCase().includes(searchTerm.toLowerCase());
      const statusMap = { Pending: 0, Approved: 1, Rejected: 2 };
      const matchesStatus =
        statusFilter === "All" || c.status === statusMap[statusFilter];
      return matchesSearch && matchesStatus;
    });
  }, [searchTerm, statusFilter, charities]);

  if (loading)
    return (
      <div className="h-[80vh] flex flex-col items-center justify-center text-brand-accent font-black">
        <Loader2 className="animate-spin mb-4" size={48} />
        <p className="tracking-widest opacity-50 uppercase">
          Synchronizing Systems...
        </p>
      </div>
    );

  return (
    <div className="space-y-8 animate-in fade-in duration-700">
      <header className="flex justify-between items-end">
        <div>
          <div className="flex items-center gap-2 text-brand-accent mb-2">
            <ShieldCheck size={20} />
            <span className="text-xs font-black uppercase tracking-[0.3em]">
              Compliance Unit
            </span>
          </div>
          <h1 className="text-4xl font-black text-white italic">
            Charity Control <span className="text-brand-accent">.</span>
          </h1>
        </div>
      </header>
      {/* Filters Section */}
      <div className="flex flex-col md:flex-row gap-4">
        <div className="relative flex-1 group">
          <Search
            className="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500"
            size={20}
          />
          <input
            type="text"
            placeholder="Search by name or license..."
            className="w-full bg-brand-card/50 border border-white/5 rounded-2xl py-4 pl-12 pr-4 text-white focus:border-brand-accent/50 outline-none transition-all shadow-xl"
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        <div className="flex gap-2 bg-brand-card/30 p-1.5 rounded-2xl border border-white/5 backdrop-blur-md">
          {["All", "Pending", "Approved", "Rejected"].map((s) => (
            <button
              key={s}
              onClick={() => setStatusFilter(s)}
              className={`px-6 py-2 rounded-xl font-bold text-sm transition-all ${statusFilter === s ? "bg-brand-accent text-brand-dark" : "text-slate-500 hover:text-white"}`}
            >
              {s}
            </button>
          ))}
        </div>
      </div>

      {/* Table Section */}
      <div className="glass-card rounded-[2.5rem] overflow-hidden border border-white/5 shadow-2xl">
        <table className="w-full text-left">
          <thead className="bg-white/5 text-slate-500 text-[10px] font-black uppercase tracking-[0.2em]">
            <tr>
              <th className="p-8">Organization Details</th>
              <th className="p-8">License</th>
              <th className="p-8 text-center">Status</th>
              <th className="p-8 text-right">Verification Control</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-white/5">
            {filteredData.map((c) => (
              <tr
                key={c.id}
                className="hover:bg-brand-accent/[0.03] transition-colors group"
              >
                <td className="p-8">
                  <div className="flex items-center gap-5">
                    <div className="w-14 h-14 rounded-2xl bg-gradient-to-br from-slate-800 to-slate-900 border border-white/5 text-brand-accent flex items-center justify-center font-black text-xl shadow-lg">
                      {c.charityName[0]}
                    </div>
                    <div>
                      <p className="text-white font-bold text-lg leading-tight mb-1">
                        {c.charityName}
                      </p>
                      <div className="flex items-center gap-2 text-slate-500 text-xs font-medium">
                        <Mail size={12} className="text-brand-accent" />{" "}
                        {c.user?.email || "pending-verification@aoun.com"}
                      </div>
                    </div>
                  </div>
                </td>
                <td className="p-8">
                  <div className="flex items-center gap-2 text-slate-400 font-mono text-sm bg-white/5 px-3 py-1.5 rounded-lg w-fit border border-white/5">
                    <Hash size={14} className="text-brand-accent" />{" "}
                    {c.licenseNumber}
                  </div>
                </td>
                <td className="p-8 text-center">
                  <span
                    className={`px-4 py-1.5 rounded-full text-[10px] font-black uppercase border tracking-widest ${
                      c.status === 0
                        ? "bg-orange-500/10 text-orange-400 border-orange-500/20"
                        : c.status === 1
                          ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20"
                          : "bg-rose-500/10 text-rose-400 border-rose-500/20"
                    }`}
                  >
                    {c.status === 0
                      ? "Pending"
                      : c.status === 1
                        ? "Approved"
                        : "Rejected"}
                  </span>
                </td>
                <td className="p-8">
                  <div className="flex justify-end gap-3 opacity-0 group-hover:opacity-100 translate-x-4 group-hover:translate-x-0 transition-all duration-300">
                    {/* زرار التواصل المباشر */}
                    <button
                      onClick={() =>
                        (window.location.href = `mailto:${c.user?.email}?subject=AOUN Verification Status`)
                      }
                      className="p-3 bg-blue-500/10 text-blue-500 rounded-xl hover:bg-blue-500 hover:text-white transition-all shadow-lg shadow-blue-500/20"
                      title="Contact via Email"
                    >
                      <Mail size={18} />
                    </button>

                    {c.status === 0 && (
                      <>
                        <button
                          onClick={() => handleAction(c.id, "approve")}
                          className="p-3 bg-emerald-500/10 text-emerald-500 rounded-xl hover:bg-emerald-500 hover:text-white transition-all shadow-lg shadow-emerald-500/20"
                        >
                          <Check size={18} />
                        </button>
                        <button
                          onClick={() => handleAction(c.id, "reject")}
                          className="p-3 bg-rose-500/10 text-rose-500 rounded-xl hover:bg-rose-500 hover:text-white transition-all shadow-lg shadow-rose-500/20"
                        >
                          <X size={18} />
                        </button>
                      </>
                    )}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
